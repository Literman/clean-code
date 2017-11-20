using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Markdown
{
    public class Md
    {
        public string RenderToHtml(string markdown)
        {
            var text = markdown.Split('\n');

            for (var i = 0; i < text.Length; i++)
                text[i] = Parse(text[i]);

            return string.Join("\n", text);
        }

        private static string Parse(string line)
        {
            var parser = new Parser();
            return parser.RenderToHtml(line);
        }
    }

    [TestFixture]
    public class Md_ShouldRender
    {
        [TestCase("abcd", ExpectedResult = "abcd")]
        [TestCase("ab cd", ExpectedResult = "ab cd")]
        [TestCase(@"\ab \cd", ExpectedResult = "ab cd")]
        public string WithoutUnderscore(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase("_qwertyqwerty_", ExpectedResult = "<em>qwertyqwerty</em>", TestName = "When 1 word in tag")]
        [TestCase("_hello world_", ExpectedResult = "<em>hello world</em>", TestName = "When 2 words in tag")]
        [TestCase("_hello_ world", ExpectedResult = "<em>hello</em> world", TestName = "When 1 word in and 1 word out of tag")]

        [TestCase("a_b", ExpectedResult = "a_b", TestName = "When only 1 underscore")]
        [TestCase("_1_23_3_", ExpectedResult = "<em>1_23_3</em>", TestName = "When number")]
        [TestCase("a_ bc _a_", ExpectedResult = "a_ bc <em>a</em>", TestName = "When space after opened underscore")]
        [TestCase("a _bc _ a_", ExpectedResult = "a <em>bc _ a</em>", TestName = "When space before closed underscore")]
        [TestCase("a_bc_a_", ExpectedResult = "a_bc_a_", TestName = "When there is no separator space between words")]

        [TestCase(@"\_a _b_ c", ExpectedResult = "_a <em>b</em> c", TestName = "When screened")]
        [TestCase(@"\_a \_ _b_ \c", ExpectedResult = @"_a _ <em>b</em> c", TestName = "When screened2")]
        [TestCase(@"\_\a\_b_c", ExpectedResult = @"_a_b_c", TestName = "When screened3")]
        public string WithSingleUnderscore(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase("__qwertyqwerty__", ExpectedResult = "<strong>qwertyqwerty</strong>", TestName = "When 1 word in tag")]
        [TestCase("__hello world__", ExpectedResult = "<strong>hello world</strong>", TestName = "When 2 words in tag")]
        [TestCase("__hello__ world", ExpectedResult = "<strong>hello</strong> world", TestName = "When 1 word in and 1 word out of tag")]

        [TestCase("a__b", ExpectedResult = "a__b", TestName = "When only 1 underscore")]
        [TestCase("__1__23__3__", ExpectedResult = "<strong>1__23__3</strong>", TestName = "When number")]
        [TestCase("a__ bc __a__", ExpectedResult = "a__ bc <strong>a</strong>", TestName = "When space after opened underscore")]
        [TestCase("a __bc __ a__", ExpectedResult = "a <strong>bc __ a</strong>", TestName = "When space before closed underscore")]
        [TestCase("a__bc__a__", ExpectedResult = "a__bc__a__", TestName = "When there is no separator space between words")]

        [TestCase(@"\__a __b__ c", ExpectedResult = "__a <strong>b</strong> c", TestName = "When screened")]
        [TestCase(@"\__a \__ __b__ \c", ExpectedResult = @"__a __ <strong>b</strong> c", TestName = "When screened2")]
        [TestCase(@"\__\a\__b__c", ExpectedResult = @"__a__b__c", TestName = "When screened3")]       
        public string WithDoubleUnderscore(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase("___qwertyqwerty___", ExpectedResult = "___qwertyqwerty___", TestName = "Ignore more than 2 underscores")]
        [TestCase("_a __qwertyqwerty__ a_", ExpectedResult = "<em>a __qwertyqwerty__ a</em>", TestName = "When double inside single")]
        [TestCase("__a _qwertyqwerty_ a__", ExpectedResult = "<strong>a <em>qwertyqwerty</em> a</strong>", TestName = "When single inside double")]

        [TestCase("__hello__ _world_", ExpectedResult = "<strong>hello</strong> <em>world</em>")]
        [TestCase("_ __hello__ _world", ExpectedResult = "_ <strong>hello</strong> _world")]
        [TestCase("__ _hello_ __world", ExpectedResult = "__ <em>hello</em> __world")]

        [TestCase(@"\__a_", ExpectedResult = @"_<em>a</em>", TestName = "When first symbol screened")]
        [TestCase(@"b\__a_", ExpectedResult = @"b__a_", TestName = "When non first symbol screened")]
        public string WithBothUnderscoresType(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase(10000), Timeout(300)]
        [TestCase(100000)]
        [TestCase(1000000)]
        public void Perfomance(int count)
        {
            var longStr = GetLongString(count);
            var markdown = longStr + " _a_";
            var expectated = longStr + " <em>a</em>";
            
            new Md().RenderToHtml(markdown).ShouldBeEquivalentTo(expectated);
        }

        private static string GetLongString(int count)
        {
            var str = new StringBuilder();
            for (var i = 0; i < count; i++)
                str.Append('a');

            return str.ToString();
        }
    }
}