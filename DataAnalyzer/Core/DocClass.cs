using System;
using System.Collections.Generic;
using System.Linq;
using DataAnalyzer.SearchRules;
namespace DataAnalyzer.Core
{
    public class DocClass : ICloneable
    {
        public DocClass(string title, int maxPagesCount = 1)
        {
            Title = title;
            MaxPagesCount = maxPagesCount;
            HeaderRules = new List<Rule>();
            FooterRules = new List<Rule>();
            DataRules = new List<Rule>();
        }

        public string Title { get; private set; }
        public int MaxPagesCount { get; set; }
        public List<Rule> HeaderRules { get; private set; }
        public List<Rule> FooterRules { get; private set; }
        public List<Rule> DataRules { get; private set; }
        public void AddHeaderRule(Rule rule)
        {
            HeaderRules.Add(rule);
            rule.Parent = HeaderRules;
        }
        public void AddDataRule(Rule rule)
        {
            DataRules.Add(rule);
            rule.Parent = DataRules;
        }        
        public void AddFooterRule(Rule rule)
        {
            FooterRules.Add(rule);
            rule.Parent = FooterRules;
        }

        public object Clone()
        {
            var head = CloneWDependencies(HeaderRules);
            var footer = CloneWDependencies(FooterRules);
            var data = CloneWDependencies(DataRules);
            return new DocClass(Title, MaxPagesCount) 
            { 
                HeaderRules = head, 
                FooterRules = footer, 
                DataRules = data,
            };
        }
        public List<Rule> CloneWDependencies(List<Rule> rules)
        {
            var result = new List<Rule>();
            foreach (var rule in rules)
            {
                if (rule.DependencyRule == null)
                    result.Add((Rule)rule.Clone());
                else
                {
                    var depName = rule.DependencyRule.Title;
                    var clonedRule = (Rule)rule.Clone();
                    clonedRule.DependencyRule = result.FirstOrDefault(r => r.Title == depName);
                    result.Add(clonedRule);
                }

            }
            return result;
        }
    }
}
