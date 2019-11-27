using DataAnalyzer.Core;
using DataAnalyzer.SearchRules;

namespace DataAnalyzer
{
    public class DocClassesCollector
    {
        public static DocClass Tawunia()
        {
            var result = new DocClass("Tawunia", 2);

            var crNumRule = new StaticTextRule("CrNum", RuleBinding.Optional);
            result.AddHeaderRule(crNumRule);
            crNumRule.TextToSearch = "CR number";
            crNumRule.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var InceptionRule = new StaticTextRule("Inception", RuleBinding.Required);
            result.AddHeaderRule(InceptionRule);
            InceptionRule.DependencyRule = crNumRule;
            var depAr = new DependencyArea();
            InceptionRule.DependencyArea = depAr;
            depAr.Below.Type = RelationType.Bot;
            depAr.RightOf.Type = RelationType.Left;
            depAr.RightOf.Offset = -100;
            depAr.LeftOf.Type = RelationType.Right;
            depAr.LeftOf.Offset = 100;
            depAr.Above.Type = RelationType.Right;
            depAr.Above.Offset = 150;
            InceptionRule.TextToSearch = "Inception";
            InceptionRule.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var PolicyRule = new StaticTextRule("PolicyRule", RuleBinding.Required);
            result.AddFooterRule(PolicyRule);
            PolicyRule.TextToSearch = "Policy Holder Confirmation";
            PolicyRule.SearchArea = new System.Windows.Rect(0, 0, 1000, 2000);

            AddDataRules(result);
            SecondPageFields(result);
            return result;
        }

        private static void AddDataRules(DocClass result)
        {
            var crNumInit = new StaticTextRule("CrNum", RuleBinding.Optional);
            result.AddDataRule(crNumInit);
            crNumInit.TextToSearch = "CR number";
            crNumInit.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var InterceptInit = new StaticTextRule("Inception", RuleBinding.Required);
            result.AddDataRule(InterceptInit);
            InterceptInit.DependencyRule = crNumInit;
            var depArinter = new DependencyArea();
            InterceptInit.DependencyArea = depArinter;
            depArinter.Below.Type = RelationType.Bot;
            depArinter.RightOf.Type = RelationType.Left;
            depArinter.RightOf.Offset = -100;
            depArinter.LeftOf.Type = RelationType.Right;
            depArinter.LeftOf.Offset = 100;
            depArinter.Above.Type = RelationType.Right;
            depArinter.Above.Offset = 150;
            InterceptInit.TextToSearch = "Inception";
            InterceptInit.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var PolicyHold = new StaticTextRule("PolicyHold", RuleBinding.Required);
            result.AddDataRule(PolicyHold);
            PolicyHold.DependencyRule = crNumInit;
            var policyDepAr = new DependencyArea();
            PolicyHold.DependencyArea = policyDepAr;
            policyDepAr.Below.Type = RelationType.Top;
            policyDepAr.Below.Offset = -150;
            policyDepAr.RightOf.Type = RelationType.Left;
            policyDepAr.RightOf.Offset = -100;
            policyDepAr.LeftOf.Type = RelationType.Right;
            policyDepAr.LeftOf.Offset = 100;
            policyDepAr.Above.Type = RelationType.Top;
            PolicyHold.TextToSearch = "Policy holder";
            PolicyHold.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var Expiry = new StaticTextRule("Expiry", RuleBinding.Required);
            result.AddDataRule(Expiry);
            Expiry.DependencyRule = InterceptInit;
            var ExpiryDepAr = new DependencyArea();
            Expiry.DependencyArea = ExpiryDepAr;
            ExpiryDepAr.Below.Type = RelationType.Bot;
            ExpiryDepAr.RightOf.Type = RelationType.Left;
            ExpiryDepAr.RightOf.Offset = -100;
            ExpiryDepAr.LeftOf.Type = RelationType.Right;
            ExpiryDepAr.LeftOf.Offset = 100;
            ExpiryDepAr.Above.Type = RelationType.Bot;
            ExpiryDepAr.Above.Offset = 150;
            Expiry.TextToSearch = "Expiry";
            Expiry.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var crNumData = new CharacterStringRule("CrNumberData", RuleBinding.Required);
            result.AddDataRule(crNumData);
            crNumData.DependencyRule = crNumInit;
            var crNumDataDR = new DependencyArea();
            crNumData.DependencyArea = crNumDataDR;
            crNumDataDR.Below.Type = RelationType.Top;
            crNumDataDR.Below.Offset = -10;
            crNumDataDR.RightOf.Type = RelationType.Right;
            crNumDataDR.RightOf.Offset = 100;
            crNumDataDR.LeftOf.Type = RelationType.Right;
            crNumDataDR.LeftOf.Offset = 700;
            crNumDataDR.Above.Type = RelationType.Bot;
            crNumDataDR.Above.Offset = 10;
            crNumData.TextToSearch = @"\d{6,12}";
            crNumData.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var InceptionData = new CharacterStringRule("InceptionData", RuleBinding.Required);
            result.AddDataRule(InceptionData);
            InceptionData.DependencyRule = InterceptInit;
            var InceptionDataDR = new DependencyArea();
            InceptionData.DependencyArea = InceptionDataDR;
            InceptionDataDR.Below.Type = RelationType.Top;
            InceptionDataDR.Below.Offset = -10;
            InceptionDataDR.RightOf.Type = RelationType.Right;
            InceptionDataDR.RightOf.Offset = 100;
            InceptionDataDR.LeftOf.Type = RelationType.Right;
            InceptionDataDR.LeftOf.Offset = 700;
            InceptionDataDR.Above.Type = RelationType.Bot;
            InceptionDataDR.Above.Offset = 10;
            InceptionData.TextToSearch = @"\d{2,4}-\d{2}-\d{2,4}";
            InceptionData.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var ExpiryData = new CharacterStringRule("ExpiryData", RuleBinding.Required);
            result.AddDataRule(ExpiryData);
            ExpiryData.DependencyRule = Expiry;
            var EpiryDataDR = new DependencyArea();
            ExpiryData.DependencyArea = EpiryDataDR;
            EpiryDataDR.Below.Type = RelationType.Top;
            EpiryDataDR.Below.Offset = -10;
            EpiryDataDR.RightOf.Type = RelationType.Right;
            EpiryDataDR.RightOf.Offset = 100;
            EpiryDataDR.LeftOf.Type = RelationType.Right;
            EpiryDataDR.LeftOf.Offset = 700;
            EpiryDataDR.Above.Type = RelationType.Bot;
            EpiryDataDR.Above.Offset = 10;
            ExpiryData.TextToSearch = @"\d{2,4}-\d{2}-\d{2,4}";
            ExpiryData.SearchArea = new System.Windows.Rect(0, 0, 1, 1);


        }

        private static void SecondPageFields(DocClass result)
        {
            var OutPatient = new StaticTextRule("Outpatient", RuleBinding.Required);
            result.AddDataRule(OutPatient);
            OutPatient.TextToSearch = "OutPatient";
            OutPatient.SearchArea = new System.Windows.Rect(0, 0, 700, 700);

            var outpatientData = new CharacterStringRule("outpatientData", RuleBinding.Required);
            result.AddDataRule(outpatientData);
            outpatientData.DependencyRule = OutPatient;
            var opDataDA = new DependencyArea();
            opDataDA.Above.Type = RelationType.Bot;
            opDataDA.Above.Offset = 20;
            opDataDA.RightOf.Type = RelationType.Right;
            opDataDA.RightOf.Offset = 20;
            opDataDA.Below.Type = RelationType.Top;
            opDataDA.Below.Offset = -20;
            opDataDA.LeftOf.Type = RelationType.Right;
            opDataDA.LeftOf.Offset = 250;
            outpatientData.DependencyArea = opDataDA;
            outpatientData.TextToSearch = @"\d{1,4}";
            outpatientData.SearchArea = new System.Windows.Rect(0, 0, 1, 1);

            var outpatientAmt = new CharacterStringRule("outpatientAmount", RuleBinding.Required);
            result.AddDataRule(outpatientAmt);
            outpatientAmt.DependencyRule = outpatientData;
            var opAmtDA = new DependencyArea();
            opAmtDA.Above.Type = RelationType.Bot;
            opAmtDA.Above.Offset = 20;
            opAmtDA.RightOf.Type = RelationType.Right;
            opAmtDA.RightOf.Offset = 20;
            opAmtDA.Below.Type = RelationType.Top;
            opAmtDA.Below.Offset = -20;
            opAmtDA.LeftOf.Type = RelationType.Right;
            opAmtDA.LeftOf.Offset = 250;
            outpatientAmt.DependencyArea = opAmtDA;
            outpatientAmt.TextToSearch = @"(\d+[\.,])+(\d{2})";
            outpatientAmt.SearchArea = new System.Windows.Rect(0, 0, 1, 1);

            var Inpatient = new StaticTextRule("Inpatient", RuleBinding.Required);
            result.AddDataRule(Inpatient);
            Inpatient.TextToSearch = "InPatient";
            Inpatient.SearchArea = new System.Windows.Rect(0, 0, 700, 700);

            var inpatientData = new CharacterStringRule("inpatientData", RuleBinding.Required);
            result.AddDataRule(inpatientData);
            inpatientData.DependencyRule = Inpatient;
            var ipData = new DependencyArea();
            ipData.Above.Type = RelationType.Bot;
            ipData.Above.Offset = 20;
            ipData.RightOf.Type = RelationType.Right;
            ipData.RightOf.Offset = 20;
            ipData.Below.Type = RelationType.Top;
            ipData.Below.Offset = -20;
            ipData.LeftOf.Type = RelationType.Right;
            ipData.LeftOf.Offset = 250;
            inpatientData.DependencyArea = ipData;
            inpatientData.TextToSearch = @"\d{1,4}";
            inpatientData.SearchArea = new System.Windows.Rect(0, 0, 1, 1);

            var inpatientAmt = new CharacterStringRule("inpatientAmt", RuleBinding.Required);
            result.AddDataRule(inpatientAmt);
            inpatientAmt.DependencyRule = inpatientData;
            var ipAmt = new DependencyArea();
            ipAmt.Above.Type = RelationType.Bot;
            ipAmt.Above.Offset = 20;
            ipAmt.RightOf.Type = RelationType.Right;
            ipAmt.RightOf.Offset = 20;
            ipAmt.Below.Type = RelationType.Top;
            ipAmt.Below.Offset = -20;
            ipAmt.LeftOf.Type = RelationType.Right;
            ipAmt.LeftOf.Offset = 250;
            inpatientAmt.DependencyArea = ipAmt;
            inpatientAmt.TextToSearch = @"(\d+[\.,])+(\d{2})";
            inpatientAmt.SearchArea = new System.Windows.Rect(0, 0, 1, 1);
        }

        public void xxx()
        {

        }
    }
}
