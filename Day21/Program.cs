using System.Numerics;

var pool = File.ReadAllLines("data.txt")
	.Aggregate(new List<Op>(), (ops, op) =>
		[.. ops, CreateOp(op)])
			.ToDictionary(x => x.Id);

var tree = pool["root"].Construct(pool);

var tree2 = tree.Evaulate();

var value = (((tree2 as BinaryOp2)?.Op1 as ConstantOp)?.Value) ?? throw new ArgumentNullException();

Console.WriteLine($"Start: {value}");


RenderTree(tree2, "");

long target = value;

Evaluate((tree2 as BinaryOp2)?.Op0 as BinaryOp2 ?? throw new ArgumentNullException());

Console.WriteLine($"Target: {target}");

void Evaluate(BinaryOp2 op)
{
	if (op.Op0 is ConstantOp constant0)
	{
		Func<long, long, long> operation = op.OperationName switch
		{
			"+" => (a, b) => a - b,
			"-" => (a, b) => b - a,
			"*" => (a, b) => a / b,
			"/" => (a, b) => b / a,
			_ => throw new ArgumentException(op.OperationName)
		};

		target = operation(target, constant0.Value);
		Console.WriteLine(target);
		if (op.Op1 is BinaryOp2 bin0)
			Evaluate(bin0);
	}
	else if (op.Op1 is ConstantOp constant)
	{
		Func<long, long, long> operation = op.OperationName switch
		{
			"+" => (a, b) => a - b,
			"-" => (a, b) => a + b,
			"*" => (a, b) => a / b,
			"/" => (a, b) => a * b,
			_ => throw new ArgumentException(op.OperationName)
		};

		target = operation(target, constant.Value);
		Console.WriteLine(target);
		if (op.Op0 is BinaryOp2 bin1)
			Evaluate(bin1);
	}
}

static void RenderTree(Op? tree, string tab)
{
	if (tree == null)
	{
		return;
	}
	
	if (tree is ConstantOp constant)
	{
		Console.WriteLine(tab + $"{constant.Id}: {constant.Value}");
		return;
	}
	else if (tree is BinaryOp2 binary)
	{
		Console.WriteLine(tab + $"{binary.Id}: {binary?.Op0?.Id} {binary?.OperationName} {binary?.Op1?.Id}");
		RenderTree(binary?.Op0, tab + "   ");
		RenderTree(binary?.Op1, tab + "   ");
	}
	else if (tree is BinaryOp binary0)
	{
		Console.WriteLine(tab + $"{binary0.Id}: {binary0.Arg0} {binary0.OperationName} {binary0.Arg1}");
		RenderTree(binary0?.Op0, tab + "   ");
		RenderTree(binary0?.Op1, tab + "   ");
	}
	else
	{
		Console.WriteLine(tab + $"{tree.Id}");
	}
}



static Op CreateOp(string s)
{
	var splitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
	var pair = s.Split(":", splitOptions);
	var id = pair.First();
	var exp = pair.Last();
	var expElements = exp.Split(" ", splitOptions);
	if (expElements.Length == 1)
	{
		return id == "humn" ?
			new StaticOp() :
			new ConstantOp(id, long.Parse(expElements.First()));
	}
	var opCharacter = expElements[1];
	Func<long, long, long> operation = opCharacter switch
	{
		"+" => (a, b) => a + b,
		"-" => (a, b) => a - b,
		"*" => (a, b) => a * b,
		"/" => (a, b) => a / b,
		_ => throw new ArgumentException(opCharacter)
	};
	return new BinaryOp(id, expElements.First(), expElements.Last(), operation, opCharacter);
}

abstract class Op(string id)
{
    public string Id { get; } = id;

	public abstract Op Construct(IDictionary<string, Op> ops);

	public abstract Op Evaulate();
}

class ConstantOp(string id, long value) : Op(id)
{
    public long Value { get; } = value;
    public override Op Construct(IDictionary<string, Op> ops)
    {
        return this;
    }

	public override Op Evaulate()
	{
		return this;
	}
}

class StaticOp() : Op("humn")
{
    public override Op Construct(IDictionary<string, Op> ops)
    {
        return this;
    }

	public override Op Evaulate()
	{
		return this;
	}
}

class BinaryOp(string id, string arg0, string arg1, Func<long, long, long> operation, string operationName) : Op(id)
{
    public string Arg0 { get; } = arg0;
    public string Arg1 { get; } = arg1;

	public Op? Op0 { get; private set; }
	public Op? Op1 { get; private set; }

    Func<long, long, long> Operation { get; } = operation;
    public string OperationName { get; } = operationName;
    public override Op Construct(IDictionary<string, Op> ops)
	{
		var arg0 = ops[Arg0];
		var arg1 = ops[Arg1];

		Op0 = arg0.Construct(ops); 
		Op1 = arg1.Construct(ops); 

		return this;
	}
	public override Op Evaulate()
	{
		var value0 = Op0!.Evaulate(); 
		var value1 = Op1!.Evaulate();

		if (value0 is ConstantOp constant0 && value1 is ConstantOp constant1)
		{
			return new ConstantOp(id, operation(constant0.Value, constant1.Value));
		}
		else if (value0 is ConstantOp constant && value1 is StaticOp staticValue)
		{
			return new BinaryOp2(id, constant, staticValue, operation, operationName);
		}
		else if (value1 is ConstantOp constant3 && value0 is StaticOp staticValue2)
		{
			return new BinaryOp2(id, staticValue2, constant3, operation, operationName);
		}
		else if (value0 is ConstantOp constant4 && value1 is BinaryOp2 binary1)
		{
			return new BinaryOp2(id, constant4, binary1, Operation, OperationName);
		}
		else if (value1 is ConstantOp constant5 && value0 is BinaryOp2 binary2)
		{
			return new BinaryOp2(id, binary2, constant5, Operation, OperationName);
		}

		throw new ArgumentException();
	}
}

class BinaryOp2(string id, Op op0, Op op1, Func<long, long, long> operation, string operationName) : Op(id)
{
    public override Op Construct(IDictionary<string, Op> ops) =>
		throw new NotImplementedException();

    public Func<long, long, long> Operation { get; } = operation;

    public string OperationName { get; } = operationName;

	public Op? Op0 { get; private set; } = op0;
	public Op? Op1 { get; private set; } = op1;

	public override Op Evaulate()
	{
		throw new ArgumentException();
	}
}