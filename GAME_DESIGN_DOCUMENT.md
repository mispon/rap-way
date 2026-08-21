# Rap Way — Game Design Document

Version: 0.1  
Status: Living document  
Last updated: 2026-08-21

## 1. Document Purpose

This document describes the agreed game vision and the systemic foundation of Rap Way. It is the product source of truth for gameplay discussions, prototypes, feature design, and prioritization.

The document intentionally separates accepted principles from balance hypotheses. Exact numbers, content catalogs, and individual event chains will be designed and tested later. When implementation or playtesting disproves a hypothesis, this document must be updated together with the design decision.

## 2. High Concept

Rap Way is an offline, mobile-first 2D sandbox about living the life of a person obsessed with music.

The player starts as an unknown artist with almost nothing and receives a set of interconnected activities rather than a prescribed story. They can work ordinary jobs to afford food and a microphone, create music, build relationships, chase attention, sign contracts, perform concerts, establish a label, become a legend, ruin their career, fall into poverty, and fight their way back.

The central fantasy is:

> Start as nobody, build a musical life through work and choices, and watch a unique story of ascent, failure, recovery, and legacy emerge from the simulation.

The game is not about completing a fixed campaign or winning isolated musical mini-games. It is about managing time, needs, craft, opportunity, people, risk, and the consequences of public attention.

## 3. Design Pillars

### 3.1 A life, not a level ladder

There is no mandatory sequence from poor beginner to celebrity. Wealth, fame, artistic respect, relationships, health, and happiness may move in different directions. Poverty and failure remain playable states rather than hard game-over screens.

### 3.2 Preparation creates outcomes

Tracks, clips, albums, concerts, publicity campaigns, and contracts are projects. Their results follow from preparation, skills, collaborators, resources, timing, and context. Player reflexes and mandatory mini-games do not replace the simulation.

### 3.3 Attention amplifies; it does not create quality

Fans, hype, relationships, and past success increase reach and opportunity. They never make unfinished work good. Greater attention magnifies both success and failure.

### 3.4 Characters remember and act independently

NPCs have careers, goals, relationships, and histories. The industry continues to change whenever the player advances game time. Important characters are not static vendors.

### 3.5 Failure produces stories

Bad decisions should create recoverable complications, new constraints, and changed relationships. Severe consequences follow readable chains rather than arbitrary punishment.

### 3.6 Depth through interaction, not interface volume

A small set of systems should combine into many outcomes. Routine actions become automatable as the career grows. Realism is abstracted when additional detail would only create repetitive management.

## 4. Audience, Tone, and Platforms

- Primary platform: mobile devices.
- Possible later platform: PC.
- Presentation: screen-based 2D interfaces, illustrated locations, character portraits, project views, news, charts, and stylized event scenes.
- Intended audience: older teenagers and adults, approximately 16+ in tone.
- Tone: mature, sometimes harsh and darkly funny, without graphic naturalism.
- Heavy topics may include poverty, addiction, violence, exploitation, mental strain, and unhealthy relationships. They are shown through decisions and consequences, neither glamorized nor reduced to moral lectures.
- Full generated songs and videos are not part of the product. Music exists through names, themes, artwork, attributes, reactions, short curated audio ambience, and the player's imagination.

The commercial release uses only fictional artists, companies, platforms, and brands. Characters may draw from broad cultural archetypes but must not be one-to-one copies of identifiable real people.

## 5. Core Gameplay Loop

The recurring loop is:

1. Read the current situation: calendar, needs, finances, opportunities, relationships, news, and trends.
2. Choose an activity or schedule several routine activities.
3. Spend game time and relevant resources.
4. Resolve work, learning, creation, travel, social interaction, or rest.
5. React to contextual events and consequences.
6. Develop projects and prepare a release or performance.
7. Convert quality, timing, and attention into audience response, money, reputation, and new opportunities.
8. Pay recurring costs, maintain the character and team, then choose the next risk.

Early play is dominated by survival, skill building, and cheap creation. Mid-game adds collaborators, stronger venues, contracts, promotion, and larger projects. Late play adds institutional power, ownership, a label, legacy, and generational succession.

## 6. Time and Calendar

### 6.1 Action-driven time

Game time does not move continuously. It advances only when the player starts an action or confirms a scheduled sequence.

Examples:

- Work shift: several hours.
- Writing session: a chosen block of hours.
- Recording session: scheduled duration plus travel.
- Sleep: chosen duration.
- Meeting, date, rehearsal, concert, or trip: context-specific duration.

The interface pauses indefinitely while the player reads or decides. Activities may animate quickly, but real seconds never determine career progress.

### 6.2 Calendar commitments

Concerts, rent, debt payments, contract deadlines, releases, meetings, and personal promises occupy dates. Accepting an opportunity creates an obligation that competes with other uses of time.

Routine actions may be queued. Interruptions and important events stop the queue when a new decision is required.

### 6.3 No offline progression

Closing the application freezes the entire world. Needs, hype, debt, NPC careers, and deadlines do not advance in real time. The world lives only when the player advances it.

The game autosaves after completed actions and important decisions.

## 7. Character Foundation

### 7.1 Renewable needs

#### Energy

Energy represents physical capacity to act.

- Most meaningful activities consume energy.
- Sleep and rest restore it.
- At zero energy, the character must rest or use a risky stimulant.
- Housing, food, health, conditions, and sleep quality modify recovery.

#### Satiety

Satiety represents access to adequate food rather than a repetitive feeding mini-game.

- Food is normally consumed automatically according to the selected food budget.
- Better food improves energy and motivation recovery and resilience to negative conditions.
- If funds are insufficient, the food tier automatically falls after a warning.
- At zero funds the character may go hungry and receive escalating conditions.
- Manual food choices are reserved for meaningful situations such as dates, celebrations, pre-show preparation, or deliberate sacrifice.

#### Motivation

Motivation represents creative willingness and psychological momentum.

- Low motivation does not forbid actions.
- Forced creative work is slower, less likely to produce exceptional ideas, and may contribute to burnout.
- Flow, inspiration, hits, and masterpieces require sufficient motivation or special circumstances.
- Experiences, relationships, success, rest, and meaningful events can restore motivation.
- Repetitive work, failure, pressure, and harmful conditions reduce it.

Energy answers “can I act?” Motivation answers “what can emerge if I force myself to act?”

### 7.2 Accumulated resources

#### Money

Money pays for survival, equipment, housing, services, staff, projects, promotion, travel, and risk. It is not a score. Recurring obligations grow with the career.

#### Fans

Fans are a relatively stable audience that grows slowly, may segment by taste, and provides long-term reach and economic value. Fans can leave after betrayal, repeated weak work, neglect, or a dramatic change of identity.

#### Hype

Hype is temporary public attention.

- It decays rapidly with game time.
- Releases, teasers, clips, concerts, collaborations, controversy, and news can raise it.
- High hype magnifies the reach and consequences of a release.
- A release does not spend hype as currency.
- A strong release may extend or increase the wave; a poor release under high expectations can collapse it and damage audiences.

### 7.3 Reputation and relationships

Public reputation is contextual rather than one global morality value. The initial model distinguishes:

- Core fans.
- The wider public.
- The music industry.

The same decision may improve one group's view and damage another's.

Every significant relationship with a person or organization uses three independent dimensions:

- Disposition: personal warmth or hostility.
- Respect: recognition of talent, status, and achievement.
- Trust: belief that the character keeps promises and is a reliable partner.

A rival may dislike but respect the player. A friend may care but not trust them professionally. A label may respect commercial potential while demanding harsh protections due to low trust.

### 7.4 Skills

Skills use durable experience rather than a hard-coded cap. The progression rule derives the displayed level from experience and may support mastery beyond 100. Potential examples include vocals, lyric writing, improvisation, instruments, production, performance, charisma, negotiation, and management. The final catalog is not yet fixed.

Skills grow by doing relevant work. Learning effectiveness follows a zone of proximal challenge:

- Routine work below the character's level gives little progress.
- A manageable challenge slightly above the current level gives maximum progress.
- An incomprehensibly difficult task gives little direct mastery, poor output, and may harm motivation.
- Mentors and stronger collaborators can break difficult work into learnable parts.
- Failure can teach when the character is capable of understanding the attempt.
- Growth slows substantially near mastery.

Equipment improves output but does not directly grant expertise.

### 7.5 Skill form

Mastery does not decay, but current form can.

- Each applicable skill stores durable experience and the last meaningful practice. Current form is derived by the applicable progression rule and character state rather than reducing durable mastery.
- Form modifies effective skill within a limited range, initially hypothesized around 0.85 to 1.05.
- Performance and motor skills lose form faster than craft or theoretical skills.
- A few meaningful practice sessions restore form quickly.
- Regular preparation can create a short peak.
- The UI shows qualitative states such as peak, good, normal, rusty, and badly out of form rather than an exact multiplier.

This mechanic is a prototype hypothesis and must be tested for value versus maintenance burden.

### 7.6 Talents

Talents are rare rule-changing abilities earned through biography, not purchased from a generic tree.

- Unlock conditions combine skill, repeated behavior, and a meaningful event.
- Talents may add new choices, convert a disadvantage into an opportunity, or change how a check resolves.
- Important events may offer a choice between talents reflecting different responses.
- Unlocks are partially hidden but foreshadowed so they do not feel random.
- Ordinary percentage bonuses belong to temporary states, not talents.

### 7.7 States, buffs, and debuffs

Temporary and persistent conditions modify resources, skills, actions, and event options.

- Every effect has a source, duration or removal condition, severity, scope, and stacking policy.
- Effects belong to groups such as sleep, food, mood, health, substances, and environment.
- Only the strongest mutually exclusive state in one group applies.
- Reapplying an effect refreshes duration or raises severity instead of creating unlimited copies.
- Independent groups can combine when narratively sensible.
- The UI explains the final modifier and its sources.

Examples include sleep deprivation, injury, inspiration, love, anxiety, withdrawal, burnout, stage fright, and peak form.

### 7.8 Stimulants and dependence

Stimulants borrow energy from the future.

- Immediate benefits are followed by a crash, worse sleep, reduced recovery, or lost motivation.
- Repeated use builds tolerance and dependence through a readable sequence of symptoms.
- A risky next use is telegraphed before the player commits.
- Dependence is a long-term condition with treatment and relapse, not a random permanent punishment after one use.
- Truly permanent consequences are rare and follow repeated ignored warnings.

## 8. Lifestyle, Housing, and Equipment

### 8.1 Recurring lifestyle categories

Lifestyle is divided into separate adjustable expenses rather than one prestige tier:

- Housing.
- Food.
- Transport.
- Later, team and personal services.

Presets such as economy, comfort, and luxury may adjust several categories together. When funds are insufficient, affected categories downgrade after a clear warning.

### 8.2 Home as a base

The home is a static, upgradable base rather than a free-form decoration editor.

- Property quality controls room count, room quality, safety, sleep, storage, and guest capacity.
- Rooms enable functions such as resting, cooking, writing, recording, and hosting collaborators.
- Portable equipment belongs to the character and can move between homes.
- Built-in upgrades such as renovation, soundproofing, and security remain with the property.
- Moving costs time and money.
- Excess equipment may be stored, sold, or left uninstalled.
- Eviction creates a deadline and recovery choices rather than deleting property instantly.

At the bottom, the player may stay with relatives or friends, use temporary accommodation, or become homeless. Each option has costs, limitations, and relationship consequences but preserves a route back.

### 8.3 Equipment

Equipment unlocks actions and improves production conditions. Better tools raise the achievable result and convenience but never replace skill, motivation, or creative decisions.

## 9. World and Map

### 9.1 City structure

The main world is a stylized 2D map of a fictional city.

- The player's home and discovered points of interest appear on the map.
- Locations open dedicated screens with people, actions, services, and events.
- Travel consumes time and sometimes money.
- Transport changes travel cost, speed, reliability, and event risk.

There is no direct movement through an open world.

### 9.2 District discovery and access

Districts are revealed through work, advertisements, relationships, invitations, exploration, and events rather than formal career levels.

Once discovered, a district remains known even after career decline. Individual venues still require money, reputation, invitations, dress, contracts, or relationships. This permits rare early access without granting permanent entry to an entire elite scene.

### 9.3 Other cities

Additional cities support concerts, tours, markets, and scenes through a travel map. They do not need to become separate walkable worlds. Their scope belongs to later development.

## 10. Activities and Survival Work

Ordinary work is a background exchange of time and energy for reliable money.

- Jobs such as courier work do not require unique mini-games.
- A shift has duration, energy cost, pay, requirements, and a contextual event pool.
- Positive and negative events may create contacts, injuries, opportunities, or complications.
- Work competes directly with music, rest, relationships, and deadlines.
- Better jobs may require skills, reputation, equipment, or reliable attendance.

Starting an activity opens a short process screen rather than resolving the shift as an instant exchange. It presents the character's progress and changing needs while the player may read news, social feeds, and charts. Simulation time still advances only through explicit discrete activity steps; the presentation is never the clock. Contextual activity events may pause progress for a decision, and routine sessions may be accelerated after the player has seen them.

Resources and relevant skill experience apply for every completed activity hour. Ordinary-work payment is settled after a successful session; an interrupted shift pays only proportionally for already completed hours and never grants completion bonuses.

The intended early tension is: work a shift and safely pay for food, or risk the day on music and a possible breakthrough.

## 11. Creating Music

### 11.1 Track as a project

A track persists through multiple stages:

1. Concept: theme, mood, genre, intent, and expected audience.
2. Beat: self-produced, licensed, purchased, or created with a collaborator.
3. Lyrics: one or more writing sessions, revision, inspiration, and possible co-writing.
4. Recording: home or studio sessions affected by equipment, energy, form, and performance.
5. Production, mixing, and mastering: performed personally or purchased as project services.
6. Completion: revise, archive, abandon, or declare ready.

A project can be revisited. More time does not guarantee infinite improvement; repeated work may reach diminishing returns, lose freshness, or damage motivation.

### 11.2 Quality dimensions and uncertainty

A work has several hidden dimensions rather than one visible quality score, including writing, performance, production, originality, cohesion, and audience fit.

The player receives qualitative observations. Accuracy improves through skill, trusted collaborators, advisors, and controlled audience testing. Advisors may be mistaken, biased, flattering, or self-interested.

A snippet can test response while risking lost novelty or a mistimed hype cycle. Full market reception is known only after release.

### 11.3 Authorship and rights

Beatmakers, writers, labels, and the artist may own different shares.

- Beats may be rented, purchased exclusively, or licensed for royalties.
- Writers may be credited co-authors, paid a flat fee, or receive a revenue share.
- Secret ghostwriting preserves the public image of sole authorship but creates a serious exposure risk.
- Rights determine future royalty flows, control of masters, disputes, reissues, and inheritance.

Permanent beatmakers and ghostwriters develop chemistry and availability but may demand recognition, better terms, or stylistic influence.

## 12. Clips, Albums, and Connected Projects

Clips and albums follow the same project philosophy as tracks without generating actual video or audio.

### 12.1 Clip

A clip builds on a selected track and adds concept, budget, director or service quality, visual identity, production risk, and release timing. A successful track increases initial interest in the clip; a weak clip can waste or damage that opportunity.

### 12.2 Album

An album is more than the sum of track scores. It considers:

- Track strength.
- Cohesion and variety.
- Sequence and pacing.
- Visual identity.
- Release strategy.
- Audience expectations.

Strong songs raise potential, but poor assembly or an unfocused campaign can still produce a disappointing album.

### 12.3 Amplification rule

Past success increases reach, opportunity, budget access, and expectations. It never directly increases the quality of the next work. Failure under greater attention produces greater reputational impact.

## 13. Releases, Audience, Charts, and Catalog

### 13.1 Release lifecycle

- Launch attention depends on hype, fans, promotion, collaborators, distribution, and expectations.
- Interest decays over time.
- Strong works retain a long tail.
- Rare works become enduring classics.
- Events, memes, clips, tours, collaborations, and reissues can revive older releases.

### 13.2 Finite attention economy

Audience attention is finite.

- An artist's catalog competes with other artists and with itself.
- Additional weak tracks redistribute existing listening rather than creating independent permanent income.
- Most old tracks approach negligible activity.
- Catalog size improves stability, discovery, concert value, and legacy only when the works retain demand.

### 13.3 Royalties

Revenue is calculated periodically and split according to rights and contracts. Gross revenue may be divided among master owners, writers, beatmakers, labels, and distributors, then reduced by applicable costs.

Exact formulas remain a balance task. The design goal is that a strong catalog creates stability without eliminating the need for decisions, expenses, and current relevance.

### 13.4 Charts

Charts provide an objective view of the market and introduce artists outside the player's immediate network. Different charts may later represent releases, genres, local scenes, sales, streams, and concerts.

## 14. Concerts

A concert is a strategically prepared event, not a mandatory rhythm game.

Before the event, the player chooses:

- Venue and capacity.
- Date and ticket price.
- Scale and budget.
- Set list and pacing.
- Guests and opening acts.
- Rehearsal and promotion.
- Team and project services.

During the show, a stylized 2D presentation delivers a small number of contextual decisions. Equipment may fail, the crowd may react poorly, a guest may be late, or the character may be exhausted. Preparation, skills, talents, states, and the chosen response determine the outcome.

After the concert the game resolves attendance, satisfaction by audience group, profit, hype, fan conversion, venue relationships, team effects, and event consequences.

A successful release raises demand for a concert. It does not guarantee that an oversized or badly prepared show succeeds.

## 15. Social Media and Public Image

The player selects content intent and tone rather than writing full posts.

Possible post purposes include announcements, teasers, personal stories, fan interaction, reactions, provocations, conflicts, and spontaneous state-driven posts.

Results depend on public image, charisma, media skill, audience, timing, hype, and current events. Repeated identical content creates fatigue.

A PR manager can automate routine publishing at a chosen autonomy level. Delegation saves character time but may create bland messaging, unwanted posts, or image drift.

Platforms are fictional and may rise, change audiences, and disappear across eras.

## 16. NPCs and the Living Industry

### 16.1 Autonomous careers

Other artists create projects, release work, gain and lose hype, perform, sign contracts, collaborate, feud, age, change style, and retire without waiting for the player.

### 16.2 Unified model, dynamic simulation detail

All NPC artists follow the same systemic model, but not at the same resolution.

- Active characters near the player's story receive detailed decisions and events.
- Observed artists in relevant genres, charts, and relationships update in medium detail.
- Background artists resolve careers through larger periodic outcomes.
- Relevance is dynamic and depends on relationships, genre, fame, location, organizations, and current events.
- When a background character becomes important, their accumulated history remains valid and gains detail.

### 16.3 Authored and procedural population

The starting world combines a small authored cast with procedural characters. Authored characters provide memorable personalities and unique event material. Procedural characters allow generational renewal and unexpected stars.

Any procedural character can become central. Real artists and labels are not used without explicit licenses.

### 16.4 News hub

The news hub reports world changes in a Football Manager-like feed.

- Personal relationships, competitors, large events, and followed subjects receive priority.
- Filters and subscriptions let the player control noise.
- Charts expose important developments beyond personalized news.
- Advertising may appear as clearly marked non-interactive commercial cards.

## 17. Personal Life

Personal life is optional world breadth, not a central mandatory progression system.

- Relationships can emerge from activities, events, compatibility, and time spent together.
- Significant NPCs retain disposition, respect, and trust.
- Partners have their own goals and agency.
- Cohabitation, promises, family, and conflict can affect housing, expenses, time, motivation, public image, and creative material.
- Children are possible but never required for succession.
- People are not consumable sources of buffs; using or neglecting them changes relationships and future choices.

## 18. Team and Delegation

### 18.1 Permanent key roles

The personal team is intentionally limited to understandable roles:

- Manager: strategy, calendar, negotiations, contracts, and opportunities.
- Music producer: creative direction, recording organization, and project specialists.
- PR manager: promotion, social media, image, news, and crises.
- Concert manager: bookings, venues, tours, and show preparation.
- Bodyguard: safety, access, threat reduction, and possible conflict escalation.
- Beatmaker: recurring music creation and creative chemistry.
- Ghostwriter: recurring writing support, rights, credit, and secrecy risks.

A player-owned label adds:

- Label director: daily company operation, roster development, and release oversight.

The final list may be reduced further after UI prototyping.

### 18.2 Project services

Lawyers, accountants, engineers, mastering specialists, video directors, stylists, photographers, instrumentalists, dancers, and other specialists are purchased for a particular need or project. They have price, quality, reputation, availability, and sometimes relationship context, but do not require continuous personnel management.

### 18.3 Autonomy

Permanent staff can operate at different autonomy levels:

- Approval required.
- Work within an agreed strategy.
- Full operational freedom.

Greater control consumes more character time. Greater autonomy creates parallel capacity and risk. Quality depends on staff skill, workload, trust, clarity, and relationships. A large team requires management capability.

## 19. Labels and Contracts

### 19.1 External labels

Signing is optional. A label trades capital, reach, expertise, and speed for rights, revenue, deadlines, and control.

Contracts may define:

- Advance and project budget.
- Revenue split.
- Duration or required releases.
- Master ownership.
- Creative approval.
- Exclusivity.
- Marketing commitments.
- Recoupment.
- Termination conditions.

Negotiation depends on respect, trust, hype, proven audience, alternatives, and skills. Independent release remains viable but transfers workload and risk to the player.

### 19.2 Player-owned label

A successful character may establish a label.

- Label finances are separate from personal money.
- The owner receives salary or dividends and cannot freely empty company funds.
- Signed artists remain autonomous NPCs.
- The player chooses contracts, budgets, staff, strategy, and levels of creative control.
- Label reputation includes commercial performance, reliability, artist treatment, and creative prestige.
- A controlling label gains predictability but may lose trust and access to strong artists.
- The label continues autonomously after the player changes protagonist.

## 20. Aging, Retirement, and Generational Play

All characters age, and the industry changes generations.

- Age changes recovery, health risk, audience expectations, roles, and opportunities rather than applying one universal penalty.
- Artists may burn out early, retire at a peak, reinvent themselves, become mentors, produce, or run organizations.
- There is no universal mandatory retirement age.
- The player may irreversibly transfer control to another character at any time.
- Forced succession occurs only through death, complete incapacity, or an optional finite-career mode.

The successor may be a relative, protégé, team member, label artist, or unrelated newcomer. The world and its history persist.

Inheritance does not grant automatic victory:

- Fans and hype belong to the previous artist.
- Assets transfer only through valid personal or legal relationships.
- A famous connection opens doors while creating expectations and accusations of favoritism.
- Skills never transfer.

Once control transfers, the former protagonist becomes an autonomous NPC. The active timeline cannot switch back. A milestone save may be loaded to create an alternate history.

## 21. Start Templates

New characters begin weak but may choose curated circumstances rather than a rigid class.

Templates use the same simulation rules and change only resources, relationships, location, skills, obligations, and states.

Possible templates include:

- On Your Own: minimal support and a clean slate.
- At Rock Bottom: no stable home, hunger, debt, and at least one viable route to work.
- Privileged Start: money and contacts, weak respect, and high expectations.
- One-Meme Wonder: high short-term hype without skill or loyal fans.
- Basement Genius: production talent with poor social or performance ability.
- Former Group Member: existing ability and audience plus conflict and disputed rights.
- Protégé: access to a previous protagonist with the burden of their shadow.

A custom start constructor may be added later. Difficulty arises from explicit world conditions rather than hidden multipliers.

## 22. Random Events

This version records only the foundation; specific event libraries will be designed separately.

- Events enter the pool through context: location, date, relationships, states, career, projects, and recent actions.
- Most create a situation and choice rather than applying an instant punishment.
- Severe outcomes usually follow warnings and a decision chain.
- Skills, talents, relationships, and resources unlock responses or change likelihood.
- Risks are described qualitatively rather than exposing every probability.
- Cooldowns, one-time flags, and memory prevent repetition and contradictions.
- Outcomes cannot be rerolled freely by reloading.

## 23. Failure and Recovery

Bankruptcy, loss of reputation, eviction, weak releases, broken relationships, and dependence are states inside the game.

- The player always retains at least one difficult path to food, shelter, work, or help unless the biography has legitimately ended.
- Recovery consumes time and may require accepting worse work, housing, contracts, or social obligations.
- Former success can help through skills and history while hurting through expectations, expenses, and public scrutiny.
- The game must avoid soft locks where every available action requires a resource the player cannot obtain.

## 24. Monetization

Rap Way is intended as a free mobile game with restrained advertising that never modifies gameplay.

- No paid energy, currency, hype, progression boosts, or rewarded ads.
- No ads tied to travel, actions, failure, or resource recovery.
- Rare interstitials may appear only at natural boundaries such as a completed period recap, with strict frequency limits.
- Clearly marked ad cards may appear in appropriate non-gameplay surfaces such as mail or news.
- Ad failure never blocks play.
- A one-time purchase may remove advertising permanently.
- A potential PC version contains no advertising.

Exact implementation, consent, privacy, regional policy, and ad frequency require separate release planning.

## 25. First Vertical Slice

The first playable slice proves the story “from courier to a filled local club.”

### Included

- One starting district.
- Home, ordinary job, shop, cheap studio, and small venue.
- Action-driven time and calendar.
- Money, energy, satiety, and motivation.
- A small initial skill set: lyric writing, performance, production, and charisma.
- First equipment purchase and one home improvement path.
- One complete track pipeline: concept, beat, lyrics, recording, mixing, and release.
- Fans, hype, qualitative quality feedback, and a simplified attention model.
- A small autonomous NPC cast with basic relationships.
- Local news and a chart.
- Work shifts with simple contextual events.
- Preparation and resolution of one concert.
- Save and load.

### Explicitly deferred

- Albums and clips.
- Full team management.
- External and player-owned labels.
- Multiple districts and cities.
- Deep dependence and treatment.
- Aging and generational succession.
- Broad personal-life simulation.
- Full advertising integration.

### Validation goal

Starting with no audience and little money, the player should face meaningful tradeoffs, create a track they can care about, release it under uncertainty, and attempt to fill a small venue. The slice succeeds if this short arc creates stories, tension, understandable consequences, and a desire to begin another career.

## 26. Open Design Work

The following subjects require dedicated design or prototypes:

- Exact formulas and pacing for all resources.
- Health, injury, aging, death, and treatment.
- Final skill list and progression curves.
- Talent catalog and unlock grammar.
- Audience segmentation, tastes, genres, and trend simulation.
- Hidden work-quality model and feedback language.
- Track, clip, album, and concert production parameters.
- NPC decision model and simulation tiers.
- Contract generation, rights accounting, and label economy.
- News selection, event authoring tools, and event libraries.
- Save versioning, deterministic randomness, and alternate timelines.
- Accessibility, localization, content controls, and final age ratings.
- Ad cadence and policy-compliant implementation.

## 27. Non-Goals

- No open-world character movement.
- No mandatory rhythm, delivery, food, or social-media mini-games.
- No real-time or offline progression.
- No unique full-song or full-video generation.
- No real artist or label simulation without licenses.
- No pay-to-win, rewarded progression, or gameplay resources sold for money.
- No requirement to manage every specialist as a permanent employee.
- No guarantee that every agreed long-term system appears in the first release.
