using System;
using System.Collections.Generic;
using System.Globalization;
using RapWay.Domain.Common;
using RapWay.Domain.Random;
using RapWay.Domain.State;
using RapWay.Domain.Time;
using RapWay.Infrastructure.Persistence.Dto;

namespace RapWay.Infrastructure.Persistence
{
    public sealed class GameStateSnapshotMapper
    {
        public const int CurrentSchemaVersion = 1;

        public GameSaveDto Capture(GameState state, DateTime savedAtUtc)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (savedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Save timestamps must be UTC.", nameof(savedAtUtc));
            }

            IReadOnlyList<RandomStreamState> capturedStreams = state.Random.CaptureStreams();
            List<RandomStreamState> sortedStreams = new(capturedStreams);
            sortedStreams.Sort((left, right) => left.Name.CompareTo(right.Name));

            List<RandomStreamStateDto> streamDtos = new(sortedStreams.Count);
            for (int index = 0; index < sortedStreams.Count; index++)
            {
                RandomStreamState stream = sortedStreams[index];
                streamDtos.Add(new RandomStreamStateDto
                {
                    Name = stream.Name.Value,
                    AlgorithmVersion = stream.AlgorithmVersion,
                    State = stream.State.ToString(CultureInfo.InvariantCulture)
                });
            }

            GameDate startDate = state.Calendar.StartDate;
            return new GameSaveDto
            {
                SchemaVersion = CurrentSchemaVersion,
                SavedAtUtc = savedAtUtc.ToString("O", CultureInfo.InvariantCulture),
                State = new GameStateDto
                {
                    Revision = state.Revision,
                    Calendar = new CalendarStateDto
                    {
                        StartYear = startDate.Year,
                        StartMonth = startDate.Month,
                        StartDay = startDate.Day,
                        StartHour = startDate.Hour,
                        TotalHours = state.Calendar.TotalHours
                    },
                    Random = new RandomStateDto
                    {
                        MasterSeed = state.Random.MasterSeed.ToString(CultureInfo.InvariantCulture),
                        Streams = streamDtos
                    }
                }
            };
        }

        public DecodedGameSave Restore(GameSaveDto save)
        {
            if (save == null)
            {
                throw new ArgumentNullException(nameof(save));
            }

            if (save.SchemaVersion != CurrentSchemaVersion)
            {
                throw new UnsupportedSaveSchemaException(save.SchemaVersion);
            }

            if (!DateTime.TryParseExact(
                    save.SavedAtUtc,
                    "O",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out DateTime savedAtUtc) || savedAtUtc.Kind != DateTimeKind.Utc)
            {
                throw new FormatException("The save timestamp must be an ISO-8601 UTC value.");
            }

            if (!ulong.TryParse(save.State.Random.MasterSeed, NumberStyles.None, CultureInfo.InvariantCulture, out ulong masterSeed))
            {
                throw new FormatException("The random master seed is invalid.");
            }

            List<RandomStreamState> streams = new(save.State.Random.Streams.Count);
            for (int index = 0; index < save.State.Random.Streams.Count; index++)
            {
                RandomStreamStateDto streamDto = save.State.Random.Streams[index] ??
                                                   throw new FormatException("A random stream entry is missing.");
                if (!ulong.TryParse(streamDto.State, NumberStyles.None, CultureInfo.InvariantCulture, out ulong streamState))
                {
                    throw new FormatException($"Random stream '{streamDto.Name}' has an invalid state.");
                }

                streams.Add(RandomStreamState.Restore(
                    StableId.Create(streamDto.Name),
                    streamDto.AlgorithmVersion,
                    streamState));
            }

            CalendarState calendar = new(
                new GameDate(
                    save.State.Calendar.StartYear,
                    save.State.Calendar.StartMonth,
                    save.State.Calendar.StartDay,
                    save.State.Calendar.StartHour),
                save.State.Calendar.TotalHours);
            RandomState random = RandomState.Restore(masterSeed, streams);
            return new DecodedGameSave(new GameState(calendar, random, save.State.Revision), savedAtUtc);
        }
    }
}
