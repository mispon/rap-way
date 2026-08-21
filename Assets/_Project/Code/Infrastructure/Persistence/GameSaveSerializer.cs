using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RapWay.Domain.State;
using RapWay.Infrastructure.Persistence.Dto;

namespace RapWay.Infrastructure.Persistence
{
    public sealed class GameSaveSerializer
    {
        public const string ChecksumAlgorithm = "sha256";

        private readonly GameStateSnapshotMapper _mapper;
        private readonly JsonSerializer _serializer;

        public GameSaveSerializer()
            : this(new GameStateSnapshotMapper())
        {
        }

        public GameSaveSerializer(GameStateSnapshotMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture,
                DateParseHandling = DateParseHandling.None,
                MissingMemberHandling = MissingMemberHandling.Error,
                NullValueHandling = NullValueHandling.Include,
                TypeNameHandling = TypeNameHandling.None
            });
        }

        public string Serialize(GameState state, DateTime savedAtUtc)
        {
            GameSaveDto save = _mapper.Capture(state, savedAtUtc);
            JObject payload = JObject.FromObject(save, _serializer);
            string checksum = ComputeChecksum(payload);
            JObject envelope = new(
                new JProperty("checksumAlgorithm", ChecksumAlgorithm),
                new JProperty("checksum", checksum),
                new JProperty("payload", payload));
            return envelope.ToString(Formatting.Indented);
        }

        public DecodedGameSave Deserialize(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new InvalidDataException("Save file is empty.");
            }

            JObject envelope;
            try
            {
                using StringReader stringReader = new(json);
                using JsonTextReader jsonReader = new(stringReader)
                {
                    DateParseHandling = DateParseHandling.None
                };
                envelope = JObject.Load(jsonReader, new JsonLoadSettings
                {
                    DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error
                });
            }
            catch (JsonException exception)
            {
                throw new InvalidDataException("Save file is not valid JSON.", exception);
            }

            ValidateEnvelopeShape(envelope);
            string algorithm = envelope.Value<string>("checksumAlgorithm") ?? string.Empty;
            if (!string.Equals(algorithm, ChecksumAlgorithm, StringComparison.Ordinal))
            {
                throw new InvalidDataException($"Unsupported checksum algorithm '{algorithm}'.");
            }

            string expectedChecksum = envelope.Value<string>("checksum") ?? string.Empty;
            JObject payload = envelope["payload"] as JObject ??
                              throw new InvalidDataException("Save envelope has no payload object.");
            string actualChecksum = ComputeChecksum(payload);
            if (!string.Equals(expectedChecksum, actualChecksum, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("Save checksum verification failed.");
            }

            GameSaveDto save;
            try
            {
                save = payload.ToObject<GameSaveDto>(_serializer) ??
                       throw new InvalidDataException("Save payload could not be decoded.");
            }
            catch (JsonException exception)
            {
                throw new InvalidDataException("Save payload does not match the current schema.", exception);
            }

            return _mapper.Restore(save);
        }

        internal string CreateEnvelope(JObject payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            JObject envelope = new(
                new JProperty("checksumAlgorithm", ChecksumAlgorithm),
                new JProperty("checksum", ComputeChecksum(payload)),
                new JProperty("payload", payload.DeepClone()));
            return envelope.ToString(Formatting.Indented);
        }

        private static string ComputeChecksum(JObject payload)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(payload.ToString(Formatting.None));
            using SHA256 sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder result = new(hash.Length * 2);
            for (int index = 0; index < hash.Length; index++)
            {
                result.Append(hash[index].ToString("x2", CultureInfo.InvariantCulture));
            }

            return result.ToString();
        }

        private static void ValidateEnvelopeShape(JObject envelope)
        {
            HashSet<string> expected = new(StringComparer.Ordinal)
            {
                "checksumAlgorithm",
                "checksum",
                "payload"
            };

            foreach (JProperty property in envelope.Properties())
            {
                if (!expected.Remove(property.Name))
                {
                    throw new InvalidDataException($"Save envelope contains unknown field '{property.Name}'.");
                }
            }

            if (expected.Count != 0)
            {
                throw new InvalidDataException("Save envelope is missing required fields.");
            }
        }
    }
}
