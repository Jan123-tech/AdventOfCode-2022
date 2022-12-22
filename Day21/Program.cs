// Part 1 only
var pool = File.ReadAllLines("data.txt")
	.Aggregate(new List<Op>(), (ops, op) =>
		ops.Concat(new [] { CreateOp(op) }).ToList())
			.ToDictionary(x => x.Id);

var result = pool["root"].Operate(pool);

Console.WriteLine(result);

Op CreateOp(string s)
{
	var splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
	var pair = s.Split(":", splitOptions);
	var id = pair.First();
	var op = pair.Last().Split(new [] { '+', '-', '*', '/' }, splitOptions);
	if (op.Length == 1)
		return new ConstantOp(id, long.Parse(op.First()));
	var index = pair.Last().IndexOf(" ") + 1;
	var opId = pair.Last()[index];
	Func<long, long, long> operation = opId switch
	{
		'+' => (a, b) => a + b,
		'-' => (a, b) => a - b,
		'*' => (a, b) => a * b,
		'/' => (a, b) => a / b,
		_ => throw new ArgumentException()
	};
	return new BinaryOp(id, op.First(), op.Last(), operation, opId.ToString());
}

abstract class Op
{
	public string Id { get; }

	public Op(string id) => Id = id;

	public abstract string Output();

	public abstract long Operate(IDictionary<string, Op> ops);
}

class ConstantOp : Op
{
	public long Value { get; }

  public ConstantOp(string id, long value) : base(id) => Value = value;

	public override long Operate(IDictionary<string, Op> ops) => Value;

  public override string Output() => $"{Id}: {Value}";
}

class BinaryOp : Op
{
	public string Arg0 { get; }
	public string Arg1 { get; }
	Func<long, long, long> Operation { get; }

	public string OperationName { get; }

	public BinaryOp(string id, string arg0, string arg1, Func<long, long, long> operation, string operationName) : base(id)
	{
		Arg0 = arg0;
		Arg1 = arg1;
		Operation = operation;
		OperationName = operationName;
	}

	public override long Operate(IDictionary<string, Op> ops)
	{
		var arg0 = ops[Arg0];
		var arg1 = ops[Arg1];

		var val0 = arg0.Operate(ops); 
		var val1 = arg1.Operate(ops); 

		return Operation(val0, val1);
	}

	public override string Output() => $"{Id}: {Arg0} {OperationName} {Arg1}";
}