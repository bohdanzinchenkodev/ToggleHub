using ToggleHub.Application.DTOs.Flag.Evaluation;
using ToggleHub.Application.Services;
using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.UnitTests.Services
{
    [TestFixture]
    public class ConditionEvaluatorTests
    {
        private ConditionEvaluator _evaluator = null!;

        [SetUp]
        public void Setup()
        {
            _evaluator = new ConditionEvaluator();
        }

        private static FlagEvaluationContext ContextWith(params (string field, object value)[] values)
        {
            var attrs = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            foreach (var (field, value) in values)
            {
                switch (value)
                {
                    case string s:
                        attrs[field] = s;
                        break;
                    case decimal d:
                        attrs[field] = d.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case int i:
                        attrs[field] = i.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case bool b:
                        attrs[field] = b.ToString();
                        break;
                    default:
                        attrs[field] = value?.ToString();
                        break;
                }
            }

            return new FlagEvaluationContext("test-sticky", attrs);
        }


        [Test]
        public void Matches_String_Equals_ShouldReturnTrue()
        {
            var cond = new RuleCondition
            {
                Field = "username",
                FieldType = RuleFieldType.String,
                Operator = OperatorType.Equals,
                ValueString = "Alice"
            };

            var ctx = ContextWith(("username", "Alice"));

            var result = _evaluator.Matches(new RuleSet { Conditions = [cond] }, ctx);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Matches_String_Contains_ShouldReturnFalse_WhenNotContained()
        {
            var cond = new RuleCondition
            {
                Field = "email",
                FieldType = RuleFieldType.String,
                Operator = OperatorType.Contains,
                ValueString = "gmail"
            };

            var ctx = ContextWith(("email", "bob@yahoo.com"));

            var result = _evaluator.Matches(new RuleSet { Conditions = [cond] }, ctx);

            Assert.That(result, Is.False);
        }

        [Test]
        public void Matches_Number_GreaterThan_ShouldWork()
        {
            var cond = new RuleCondition
            {
                Field = "age",
                FieldType = RuleFieldType.Number,
                Operator = OperatorType.GreaterThan,
                ValueNumber = 18
            };

            var ctx = ContextWith(("age", 21m));

            var result = _evaluator.Matches(new RuleSet { Conditions = [cond] }, ctx);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Matches_Boolean_Equals_ShouldWork()
        {
            var cond = new RuleCondition
            {
                Field = "isAdmin",
                FieldType = RuleFieldType.Boolean,
                Operator = OperatorType.Equals,
                ValueBoolean = true
            };

            var ctx = ContextWith(("isAdmin", true));

            var result = _evaluator.Matches(new RuleSet { Conditions = [cond] }, ctx);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Matches_List_In_ShouldWorkForStringMatch()
        {
            var cond = new RuleCondition
            {
                Field = "role",
                FieldType = RuleFieldType.List,
                Operator = OperatorType.In,
                Items = new List<RuleConditionItem>
                {
                    new() { ValueString = "Admin" },
                    new() { ValueString = "Manager" }
                }
            };

            var ctx = ContextWith(("role", "Manager"));

            var result = _evaluator.Matches(new RuleSet { Conditions = [cond] }, ctx);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Matches_List_NotIn_ShouldReturnTrue_WhenValueNotInList()
        {
            var cond = new RuleCondition
            {
                Field = "role",
                FieldType = RuleFieldType.List,
                Operator = OperatorType.NotIn,
                Items = new List<RuleConditionItem>
                {
                    new() { ValueString = "Guest" }
                }
            };

            var ctx = ContextWith(("role", "Admin"));

            var result = _evaluator.Matches(new RuleSet { Conditions = [cond] }, ctx);

            Assert.That(result, Is.True);
        }

        [Test]
        public void Matches_MultipleConditions_AllMustMatch()
        {
            var cond1 = new RuleCondition
            {
                Field = "username",
                FieldType = RuleFieldType.String,
                Operator = OperatorType.StartsWith,
                ValueString = "Al"
            };

            var cond2 = new RuleCondition
            {
                Field = "age",
                FieldType = RuleFieldType.Number,
                Operator = OperatorType.GreaterThan,
                ValueNumber = 18
            };

            var ctx = ContextWith(("username", "Alice"), ("age", 21m));

            var result = _evaluator.Matches(new RuleSet { Conditions = [cond1, cond2] }, ctx);

            Assert.That(result, Is.True);
        }
    }
}
