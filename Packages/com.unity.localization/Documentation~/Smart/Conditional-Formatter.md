# Conditional Formatter

Conditional formatting occurs on any placeholder that contains a pipe character `|` after the colon `:`.
To invoke the [Conditional Formatter](xref:UnityEngine.Localization.SmartFormat.Extensions.ConditionalFormatter) explicitly use the name "conditional" or "cond". It can also be used implicitly when no name is provided.

The behavior of Conditional Formatter varies depending on the data type of the placeholder:

## Number

When you use a number type, such as integer or double, the value determines which item is selected from the choices list. If the value is not an integer, it is rounded down to the nearest whole number—the floor value is used as the index.

If the value is negative or exceeds the number of available choices, the default option is returned.

The following shows the example outputs of the number type:

| **Syntax** | **Value** | **Output** |
| ----------- | --------- | ---------- |
| {0:cond:Apple\|Pie\|Orange\|Banana\|No fruit} | 0 | "Apple" |
| {0:cond:Apple\|Pie\|Orange\|Banana\|No fruit} | 3 | "Banana" | 
| {0:cond:Apple\|Pie\|Orange\|Banana\|No fruit} | -1 | "No Fruit" |
| {value:cond:zero\|one\|two\|three\|other} | 0 | "zero" |
| {value:cond:zero\|one\|two\|three\|other} | 1 | "one" |
| {value:cond:zero\|one\|two\|three\|other} | 4 | "other" |

### Complex Comparisons

You can define more complex comparisons with the following syntax:

- Each parameter is separated by a `|`.
- A comparison is followed by a ?, then the corresponding output text.
- The final (default) entry does not include a comparison or a ?; it serves as the fallback if no conditions are met.

Each parameter is separated by `|`. The comparison is followed by a `?` and then the text. The last (default) entry does not contain a comparison nor a `?`.

The following comparisons are supported:

| **Operator** | **Description** |
| ------------ | ----------- |
| **&gt;=**    | greater than or equal to |
| **&gt;**     | greater than |
| **=**        | equal to |
| **&lt;**     | less than |
| **&lt;=**    | less than or equal to |
| **!=**       | not equal to |

To combine comparisons, use `&` for AND or `/` for OR.

<table>
<tr>
<th><strong>Example Smart String</strong></th>
<th><strong>Arguments</strong></th>
<th><strong>Result</strong></th>
</tr>

<tr>
<td>{0:cond:>10?Greater Than 10\|=10?Equals to 10\|Less than 10}</td>
<td><code>5</code></td>
<td>Less than 10</td>

<tr>
<td>{Age:cond:&gt;=55?Senior Citizen|&gt;=30?Adult|&gt;=18?Young Adult|&gt;12?Teenager|&gt;2?Child|Baby}</td>
<td>

[!code-cs[](../../DocCodeSamples.Tests/SmartStringSamples.cs#args-cond-1)]

</td>
<td>Adult</td>
</tr>

</table>

## Booleans

When you use a boolean data type, the value determines the output.
If the value is true, the first item in the output choices is used. If the value is false, the second item is selected.

Syntax: `{0:cond:true|false}`.

| **Syntax** | **Value** | **Output** |
| ---------- | --------- | ---------- |
| Enabled? {0:Yes\|No}. | true | "Enabled? Yes." |
| Enabled? {0:Yes\|No}. | false | "Enabled? No." |

## Strings

When you use a string data type, the value itself is output as long as it is not null or an empty string (""). If the value is null or empty, the default choice is used instead.

Syntax: `{0:cond:default|null or empty}`

| **Syntax** | **Value** | **Output** |
| ---------- | --------- | ---------- |
| The string is {0:not null or empty|null or empty} | "Some text" | "The string is not null or empty" |
| The string is {0:not null or empty|null or empty}  | "" | "The string is null or empty" |
| Text: {0:{0}\|No text to display} | "Hello World" | "Text: Hello World" |
| Text: {0:{0}\|No text to display}  | null | "No text to display" |
| Text: {0:{0}\|No text to display}  | "" | "No text to display" |

## DateTime or DateTimeOffset

When you use DateTime or DateTimeOffset data types, the value is compared to the **current calendar date** (year, month, and day only).

If there are three output choices, the index values are:

- 0 for a past date
- 1 for today
- 2 for a future date

Syntax: `{0:cond:past date|today|future date}`

If there are two output choices, the index values are:

- 0 for today or a past date
- 1 for a future date

Syntax: `{0:cond:today or past date|future date}`

| **Syntax** | **Value** | **Output** |
| ---------- | --------- | ---------- |
| My birthday {0:was yesterday\|is today\|will be tomorrow} | DateTime.Now.AddDays(1) | "My birthday was yesterday" |
| My birthday {0:was yesterday\|is today\|will be tomorrow} | DateTime.Now | "My birthday is today" |
| My birthday {0:was yesterday\|is today\|will be tomorrow} | DateTime.Now.AddDays(-1) | "My birthday will be tomorrow" |

## TimeSpan

When using the TimeSpan data type, the value is compared to `TimeSpan.Zero`.

If there are three output choices, the index values are:

- 0 for a negative duration
- 1 for zero
- 2 for a positive duration

Syntax: `{0:cond:negative duration|zero|positive duration}`

If there are two output choices, the index values are:

- 0 for a negative or zero duration
- 1 for a positive duration

Syntax: `{0:cond:negative or zero duration|positive duration}`

| **Syntax** | **Value** | **Output** |
| ---------- | --------- | ---------- |
| The event {0:cond:will start in {Hours} hours\|is now\|was {Hours} hours ago} | TimeSpan.Zero.Add(new TimeSpan(-2,0,0)) | "The event will start in 2 hours" |
| The event {0:cond:will start in {Hours} hours\|is now\|was {Hours} hours ago} | TimeSpan.Zero | "The event is now" |
| The event {0:cond:will start in {Hours} hours\|is now\|was {Hours} hours ago} | TimeSpan.Zero.Add(new TimeSpan(3,0,0)) | "The event was 3 hours ago" |

## Other (object)

If the data type is not one of the types listed above, it is treated as a general object and evaluated against null. If the value is not null, the choice at index 0 is used. If the value is null, the choice at index 1 is used instead.

Syntax: `{0:cond:not null|null}`
