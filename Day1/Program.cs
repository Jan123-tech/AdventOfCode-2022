var sum = File.ReadAllText("data.txt")
    .Split(string.Concat(Environment.NewLine, Environment.NewLine))
    .Select(x => x
        .Split(Environment.NewLine)
        .Select(x => int.Parse(x))
        .Sum())
    .OrderByDescending(x => x)
    .Take(3)
    .Sum();

Console.WriteLine(sum);