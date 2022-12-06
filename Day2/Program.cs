var wins = new HashSet<(Play, Play)>
{
    { (Play.Paper, Play.Rock) },
    { (Play.Scissors, Play.Paper) },
    { (Play.Rock, Play.Scissors) },
};

Play targetToPlay((Play, Target) pair) =>
    pair.Item2 == Target.Draw ?
        pair.Item1 :
        pair.Item2 == Target.Win ?
            wins!.First(x => x.Item2 == pair.Item1).Item1 :
            wins!.First(x => x.Item1 == pair.Item1).Item2;

Play toPlay(string s) => s == "A" ? Play.Rock : s == "B" ? Play.Paper : Play.Scissors;
Target toTarget(string s) => s == "X" ? Target.Loss : s == "Y" ? Target.Draw : Target.Win;

int roundScore((Play, Play) pair) => playScore(pair.Item2) + resultScore(pair);
int playScore(Play s) => s == Play.Rock ? 1 : s == Play.Paper ? 2 : 3;
int resultScore((Play, Play) pair) => pair.Item1 == pair.Item2 ? 3 : wins!.Contains(pair) ? 0 : 6;

var sum = File.ReadAllText("data.txt").Split(Environment.NewLine)
    .Select(x => (x.First().ToString(), x.Last().ToString()))
    .Select(x => (toPlay(x.Item1), toTarget(x.Item2)))
    .Select(x => (x.Item1, targetToPlay(x)))
    .Select(x => roundScore(x))
    .Sum();

Console.WriteLine(sum);

enum Play { Rock, Paper, Scissors }
enum Target { Loss, Draw, Win }