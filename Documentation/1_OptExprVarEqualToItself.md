## AST-оптимизация замены сравнения переменной с собой на true

### Постановка задачи
Реализовать оптимизацию по абстрактному синтаксическому дереву — замена сравнения переменной с собой на true:

- a == a => true
- a <= a => true
- a >= a => true

### Команда
Д. Володин, Н. Моздоров

### Зависимые и предшествующие задачи

Предшествующие:

- Построение AST-дерева
- Базовые визиторы
- ChangeVisitor

### Теоретическая часть

Данная оптимизация выполняется на абстрактном синтаксическом дереве, построенном для данной программы. Необходимо найти в нём узлы, содержащие операции сравнения (==, <=, >=) с одной и той же переменной, и заменить эти сравнения на True.

### Практическая часть

Нужная оптимизация производится с применением паттерна Visitor, для этого созданный класс наследует `ChangeVisitor` и
переопределяет метод `PostVisit`.

```csharp
public class OptExprVarEqualToItself : ChangeVisitor
{
    public override void PostVisit(Node n)
    {
        // Equality to itself   a == a, a <= a, a >= a
        if (n is BinOpNode binop && binop.Left is IdNode Left && binop.Right is IdNode Right &&
            Left.Name == Right.Name &&
            (binop.Op == OpType.EQUAL || binop.Op == OpType.EQLESS || binop.Op == OpType.EQGREATER))
        {
            ReplaceExpr(binop, new BoolValNode(true));
        }
    }
}
```

### Место в общем проекте (Интеграция)

Данная оптимизация применяется в классе `ASTOptimizer` наряду со всеми остальными оптимизациями по абстрактному синтаксическому дереву.

### Тесты

```csharp
[TestCase(@"
var a;
a = a == a;
",
    ExpectedResult = new[]
    {
        "var a;",
        "a = true;"
    },
    TestName = "EQUAL")]
[TestCase(@"
var a;
a = a <= a;
",
    ExpectedResult = new[]
    {
        "var a;",
        "a = true;"
    },
    TestName = "EQLESS")]
[TestCase(@"
var a;
a = a >= a;
",
    ExpectedResult = new[]
    {
        "var a;",
        "a = true;"
    },
    TestName = "EQGREATER")]
public string[] TestOptExprVarEqualToItself(string sourceCode) =>
    TestASTOptimization(sourceCode, new OptExprVarEqualToItself());
```
