using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Markdown
{
    public class Md
    {
        private List<ITagable> parsers = new List<ITagable> { new StrongTag(), new EmTag() };

        public string RenderToHtml(string markdown)
        {
            var text = markdown.Split('\n');

            for (var i = 0; i < text.Length; i++)
                text[i] = Parse(text[i]);
            
            return string.Join("\n", text);
        }

        private string Parse(string markdown)
        {
            foreach (var parser in parsers)
            {
                markdown = parser.FindAndReplace(markdown);
            }
            return markdown;
        }
    }

    [TestFixture]
    public class Md_ShouldRender
    {
        [TestCase("abcd", ExpectedResult = "abcd")]
        [TestCase("ab cd", ExpectedResult = "ab cd")]
        public string WithoutUnderscore(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase("_qwertyqwerty_", ExpectedResult = "<em>qwertyqwerty</em>", TestName = "When 1 word in tag")]
        [TestCase("_hello world_", ExpectedResult = "<em>hello world</em>", TestName = "When 2 words in tag")]
        [TestCase("_hello_ world", ExpectedResult = "<em>hello</em> world", TestName = "When 1 word in and 1 word out of tag")]

        [TestCase("a_b", ExpectedResult = "a_b", TestName = "When only 1 underscore")]
        [TestCase("_1_23_3_", ExpectedResult = "_1_23_3_", TestName = "When number")]
        [TestCase("a_ bc_a_", ExpectedResult = "a_ bc<em>a</em>", TestName = "When space after opened underscore")]
        [TestCase("a_bc _a_", ExpectedResult = "a<em>bc _a</em>", TestName = "When space before closed underscore")]

        [TestCase(@"\_a_b_c", ExpectedResult = "_a<em>b</em>c", TestName = "When screened")]
        [TestCase(@"\_a_\b_\c", ExpectedResult = @"_a<em>\b</em>\c", TestName = "When screened2")]
        [TestCase(@"\_\a\_b_c", ExpectedResult = @"_\a_b_c", TestName = "When screened3")]
        public string WithSingleUnderscore(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase("__qwertyqwerty__", ExpectedResult = "<strong>qwertyqwerty</strong>", TestName = "When 1 word in tag")]
        [TestCase("__hello world__", ExpectedResult = "<strong>hello world</strong>", TestName = "When 2 words in tag")]
        [TestCase("__hello__ world", ExpectedResult = "<strong>hello</strong> world", TestName = "When 1 word in and 1 word out of tag")]

        [TestCase("a__b", ExpectedResult = "a__b", TestName = "When only 1 underscore")]
        [TestCase("__1__23__3__", ExpectedResult = "__1__23__3__", TestName = "When number")]
        [TestCase("a__ bc__a__", ExpectedResult = "a__ bc<strong>a</strong>", TestName = "When space after opened underscore")]
        [TestCase("a__bc __a__", ExpectedResult = "a<strong>bc __a</strong>", TestName = "When space before closed underscore")]

        [TestCase(@"\__a__b__c", ExpectedResult = "__a<strong>b</strong>c", TestName = "When screened")]
        [TestCase(@"\__a__\b__\c", ExpectedResult = @"__a<strong>\b</strong>\c", TestName = "When screened2")]
        [TestCase(@"\__\a\__b__c", ExpectedResult = @"__\a__b__c", TestName = "When screened3")]
        public string WithDoubleUnderscore(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }

        [TestCase("___qwertyqwerty___", ExpectedResult = "<strong><em>qwertyqwerty</em></strong>")]
        [TestCase("_a__qwertyqwerty__a_", ExpectedResult = "<em>a__qwertyqwerty__a</em>")]
        [TestCase("__a_qwertyqwerty_a__", ExpectedResult = "<strong>a<em>qwertyqwerty</em>a</strong>")]

        [TestCase("__hello__ _world_", ExpectedResult = "<strong>hello</strong> <em>world</em>")]
        [TestCase("_ __hello__  _world", ExpectedResult = "_ <strong>hello</strong> _world")]
        [TestCase("__ _hello_ __world", ExpectedResult = "__ <em>hello</em> __world")]
        public string WithBothUnderscoresType(string markDown)
        {
            return new Md().RenderToHtml(markDown);
        }
    }
}