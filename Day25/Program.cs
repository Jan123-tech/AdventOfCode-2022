var lines = File.ReadAllLines("data.txt");

var maxLength = lines.Select(x => x.Length).Max();
var powers = Enumerable.Range(0, maxLength*2).Select((x, index) => (index, x, value: (long)Math.Pow(5, x))).ToList();

var numbers = lines.Select(x => 
{
	var snafu = x.Reverse().Select(x => x == '=' ? (long)-2 : x == '-' ? (long)-1 : long.Parse(x.ToString()));

	var snafu2 = snafu.Select((x, i) => x * powers[i].value);

	var dec = snafu2.Sum();
	
	return (SNAFU: x, base10: dec);
	
}).ToList();

var sum = numbers.Sum(x => x.base10);

Console.WriteLine(GetSnafu(sum));

string GetSnafu(long sum)
{
	var powersOrdinals = new List<(long power, long ordinal, int index)>();
	var remainder = sum;
	var powerIndex = 0;

	while (remainder > 0)
	{
		var p =  powers![powerIndex++];
		var power = p.value;
		var index = p.index;
		var multiplier = remainder / power;

		if (multiplier < 5)
		{
			powersOrdinals.Add((power, multiplier, index));
			var amount = multiplier * power;
			remainder -= amount;
			powerIndex = 0;
		}
	}

	var missing = powers.Where(x => !powersOrdinals.Select(x => x.index).Contains(x.index)).Select(x => (x.value, (long)0, x.index)).ToList();

	foreach (var m in missing)
	{
		powersOrdinals.Add(m);
	}

	var snafuStr0 = string.Join(string.Empty, powersOrdinals.OrderBy(x => x.index).Select(x => x.ordinal));
	var snafuStr = snafuStr0.TrimEnd('0');

  var newSnafu = snafuStr.Concat("0").Select(x => int.Parse(x.ToString())).Aggregate(
    (sb: new System.Text.StringBuilder(), remainder: 0), (a, digit) =>
    {
      var newValue = a.remainder + digit;

      (char newChar, int remainder) = newValue switch
      {
        < 3 => (newValue.ToString().First(), 0),
        3 => ('=', 1),
        4 => ('-', 1),
				5 => ('0', 1),
				6 => ('1', 1),
        _ => throw new ArgumentException()
			};

      a.sb.Append(newChar);

      return (a.sb, remainder);
    });

	var end = (string.Join(string.Empty, newSnafu.sb.ToString().Reverse()) ?? string.Empty);
	return end.StartsWith("0") ? end.Substring(1) : end.ToString();
}