Play ToPlay(char s) => s == 'A' ? Play.Rock : s == 'B' ? Play.Paper : Play.Scissors;
Result ToResult(char s) => s == 'X' ? Result.Loss : s == 'Y' ? Result.Draw : Result.Win;

Play PlayForResult(Play play, Result result) =>
    result == Result.Draw ? play : GetDecisivePlay(play, result);

Play GetDecisivePlay(Play play, Result result)
{
    if (result == Result.Draw)
        throw new ArgumentException();

    var playInt = (int) play;
    var inc = result == Result.Win ? 1 : 2;
    var newPlay = playInt + inc;
    var norm = newPlay % 3;

    return (Play) norm;
}

int Score(Play play1, Play play2) =>
    (int) play2 + 1 + (int) GetResult(play1, play2);

Result GetResult(Play play1, Play play2) => play1 == play2 ?
    Result.Draw :
    PlayForResult(play1, Result.Win) == play2 ?
        Result.Win :
        Result.Loss;

var sum = File.ReadAllLines("data.txt")
    .Select(x => (play: ToPlay(x.First()), result: ToResult(x.Last())))
    .Select(x => (play1: x.play, play2: PlayForResult(x.play, x.result)))
    .Select(x => Score(x.play1, x.play2))
    .Sum();

Console.WriteLine(sum);

enum Play { Rock = 0, Paper = 1, Scissors = 2 }
enum Result { Loss = 0, Draw = 3, Win = 6 }

//10835