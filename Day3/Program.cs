var sum = File.ReadAllText("data.txt").Split(Environment.NewLine)
    .Select((x, i) => (x, i))
    .GroupBy(x => x.i / 3)
    .Select(x => x
        .Select(x => x.x)
        .Skip(1)
        .Aggregate(x.First().x, (s0, s1) => string.Concat(s0.Intersect(s1)))
        .First())
    .Select(x => x - (x >= 97 ? 96 : 38))
    .Sum();

Console.WriteLine(sum);