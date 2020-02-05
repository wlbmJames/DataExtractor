using DataAnalyzer.Core;
using DataAnalyzer.SearchRules;
using DataAnalyzer.SearchRules.ConstraintsAdd;

namespace DataAnalyzer
{
    public class DocClassesCollector
    {
        #region Legacy
        #region Tawunia
        public static DocClass TawuniaLegacy()
        {
            var result = new DocClass("Tawunia", 2);

            var crNumRule = new StaticTextRule("CrNum", RuleBinding.Optional);
            result.AddHeaderRule(crNumRule);
            crNumRule.TextToSearch = "CR number";
            crNumRule.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var stBupaArabia = new StaticTextRule("Bupa Arabia", RuleBinding.Prohibited);
            result.AddHeaderRule(stBupaArabia);
            stBupaArabia.TextToSearch = "Bupa Arabia";
            stBupaArabia.SearchArea = new System.Windows.Rect(0, 0, 2000, 3000);

            var InceptionRule = new StaticTextRule("Inception", RuleBinding.Required);
            result.AddHeaderRule(InceptionRule);
            InceptionRule.DependencyRule = crNumRule;
            var depAr = new DependencyArea();
            InceptionRule.DependencyArea = depAr;
            depAr.Below.Type = RelationTypes.Bot;
            depAr.RightOf.Type = RelationTypes.Left;
            depAr.RightOf.Offset = -100;
            depAr.LeftOf.Type = RelationTypes.Right;
            depAr.LeftOf.Offset = 100;
            depAr.Above.Type = RelationTypes.Right;
            depAr.Above.Offset = 150;
            InceptionRule.TextToSearch = "Inception";
            InceptionRule.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var PolicyRule = new StaticTextRule("PolicyRule", RuleBinding.Required);
            result.AddFooterRule(PolicyRule);
            PolicyRule.TextToSearch = "Policy Holder Confirmation";
            PolicyRule.SearchArea = new System.Windows.Rect(0, 0, 1000, 2000);

            TawuniaLegacyAddDataRules(result);
            TawuniaLegacySecondPageFields(result);
            return result;
        }

        private static void TawuniaLegacyAddDataRules(DocClass result)
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
            depArinter.Below.Type = RelationTypes.Bot;
            depArinter.RightOf.Type = RelationTypes.Left;
            depArinter.RightOf.Offset = -100;
            depArinter.LeftOf.Type = RelationTypes.Right;
            depArinter.LeftOf.Offset = 100;
            depArinter.Above.Type = RelationTypes.Right;
            depArinter.Above.Offset = 150;
            InterceptInit.TextToSearch = "Inception";
            InterceptInit.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var PolicyHold = new StaticTextRule("PolicyHold", RuleBinding.Required);
            result.AddDataRule(PolicyHold);
            PolicyHold.DependencyRule = crNumInit;
            var policyDepAr = new DependencyArea();
            PolicyHold.DependencyArea = policyDepAr;
            policyDepAr.Below.Type = RelationTypes.Top;
            policyDepAr.Below.Offset = -150;
            policyDepAr.RightOf.Type = RelationTypes.Left;
            policyDepAr.RightOf.Offset = -100;
            policyDepAr.LeftOf.Type = RelationTypes.Right;
            policyDepAr.LeftOf.Offset = 100;
            policyDepAr.Above.Type = RelationTypes.Top;
            PolicyHold.TextToSearch = "Policy holder";
            PolicyHold.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var Expiry = new StaticTextRule("Expiry", RuleBinding.Required);
            result.AddDataRule(Expiry);
            Expiry.DependencyRule = InterceptInit;
            var ExpiryDepAr = new DependencyArea();
            Expiry.DependencyArea = ExpiryDepAr;
            ExpiryDepAr.Below.Type = RelationTypes.Bot;
            ExpiryDepAr.RightOf.Type = RelationTypes.Left;
            ExpiryDepAr.RightOf.Offset = -100;
            ExpiryDepAr.LeftOf.Type = RelationTypes.Right;
            ExpiryDepAr.LeftOf.Offset = 100;
            ExpiryDepAr.Above.Type = RelationTypes.Bot;
            ExpiryDepAr.Above.Offset = 150;
            Expiry.TextToSearch = "Expiry";
            Expiry.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var crNumData = new CharacterStringRule("CrNumberData", RuleBinding.Required);
            result.AddDataRule(crNumData);
            crNumData.DependencyRule = crNumInit;
            var crNumDataDR = new DependencyArea();
            crNumData.DependencyArea = crNumDataDR;
            crNumDataDR.Below.Type = RelationTypes.Top;
            crNumDataDR.Below.Offset = -10;
            crNumDataDR.RightOf.Type = RelationTypes.Right;
            crNumDataDR.RightOf.Offset = 100;
            crNumDataDR.LeftOf.Type = RelationTypes.Right;
            crNumDataDR.LeftOf.Offset = 700;
            crNumDataDR.Above.Type = RelationTypes.Bot;
            crNumDataDR.Above.Offset = 10;
            crNumData.TextToSearch = @"\d{6,12}";
            crNumData.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var InceptionData = new CharacterStringRule("InceptionDate", RuleBinding.Required);
            result.AddDataRule(InceptionData);
            InceptionData.DependencyRule = InterceptInit;
            var InceptionDataDR = new DependencyArea();
            InceptionData.DependencyArea = InceptionDataDR;
            InceptionDataDR.Below.Type = RelationTypes.Top;
            InceptionDataDR.Below.Offset = -10;
            InceptionDataDR.RightOf.Type = RelationTypes.Right;
            InceptionDataDR.RightOf.Offset = 100;
            InceptionDataDR.LeftOf.Type = RelationTypes.Right;
            InceptionDataDR.LeftOf.Offset = 700;
            InceptionDataDR.Above.Type = RelationTypes.Bot;
            InceptionDataDR.Above.Offset = 10;
            InceptionData.TextToSearch = @"\d{2,4}-\d{2}-\d{2,4}";
            InceptionData.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);

            var ExpiryData = new CharacterStringRule("ExpiryDate", RuleBinding.Required);
            result.AddDataRule(ExpiryData);
            ExpiryData.DependencyRule = Expiry;
            var EpiryDataDR = new DependencyArea();
            ExpiryData.DependencyArea = EpiryDataDR;
            EpiryDataDR.Below.Type = RelationTypes.Top;
            EpiryDataDR.Below.Offset = -10;
            EpiryDataDR.RightOf.Type = RelationTypes.Right;
            EpiryDataDR.RightOf.Offset = 100;
            EpiryDataDR.LeftOf.Type = RelationTypes.Right;
            EpiryDataDR.LeftOf.Offset = 700;
            EpiryDataDR.Above.Type = RelationTypes.Bot;
            EpiryDataDR.Above.Offset = 10;
            ExpiryData.TextToSearch = @"\d{2,4}-\d{2}-\d{2,4}";
            ExpiryData.SearchArea = new System.Windows.Rect(0, 0, 1, 1);

            //-----------------------------------TABLE
            var stPeriod = new StaticTextRule("Period", RuleBinding.Required);
            stPeriod.TextToSearch = "Period";
            result.AddDataRule(stPeriod);
            stPeriod.DependencyRule = Expiry;
            var daPeriod = new DependencyArea();
            stPeriod.DependencyArea = daPeriod;
            daPeriod.Below.Type = RelationTypes.Bot;
            daPeriod.Below.Offset = 100;
            daPeriod.RightOf.Type = RelationTypes.Left;
            daPeriod.LeftOf.Type = RelationTypes.Right;
            daPeriod.LeftOf.Offset = 200;
            daPeriod.Above.Type = RelationTypes.Bot;
            daPeriod.Above.Offset = 500;

            var csPeriod = new CharacterStringRule("PeriodData", RuleBinding.Required);
            csPeriod.TextToSearch = @"[a-zA-Z\s\d]+";
            result.AddDataRule(csPeriod);
            csPeriod.DependencyRule = stPeriod;
            var daPeriodData = new DependencyArea();
            csPeriod.DependencyArea = daPeriodData;
            daPeriodData.Below.Type = RelationTypes.Bot;
            daPeriodData.Above.Type = RelationTypes.Bot;
            daPeriodData.Above.Offset = 100;
            daPeriodData.LeftOf.Type = RelationTypes.Right;
            daPeriodData.LeftOf.Offset = 150;
            daPeriodData.RightOf.Type = RelationTypes.Left;
            daPeriodData.RightOf.Offset = -100;

            var rgPeriodData = new RepeatingCSRule("LastYear_MonthlyClaim_0", RuleBinding.Required);
            rgPeriodData.TextToSearch = @"\d{2}\/\d{4}";
            result.AddDataRule(rgPeriodData);
            rgPeriodData.DependencyRule = stPeriod;
            var daRgPeriodData = new DependencyArea();
            daRgPeriodData.Below.Type = RelationTypes.Bot;
            daRgPeriodData.Below.Offset = 20;
            daRgPeriodData.LeftOf.Type = RelationTypes.Right;
            daRgPeriodData.LeftOf.Offset = 200;
            daRgPeriodData.RightOf.Type = RelationTypes.Right;
            daRgPeriodData.RightOf.Offset = 100;
            daRgPeriodData.Above.Type = RelationTypes.Bot;
            daRgPeriodData.Above.Offset = 70;
            rgPeriodData.DependencyArea = daRgPeriodData;

            var rgPeriodNumOfLives = new RepeatingCSRule("LastYear_NumberOfLives_0", RuleBinding.Required);
            rgPeriodNumOfLives.TextToSearch = @"\d{1,4}";
            result.AddDataRule(rgPeriodNumOfLives);
            rgPeriodNumOfLives.DependencyRule = stPeriod;
            var dargPeriodNumOfLives = new DependencyArea();
            dargPeriodNumOfLives.Below.Type = RelationTypes.Bot;
            dargPeriodNumOfLives.Below.Offset = 20;
            dargPeriodNumOfLives.LeftOf.Type = RelationTypes.Right;
            dargPeriodNumOfLives.LeftOf.Offset = 350;
            dargPeriodNumOfLives.RightOf.Type = RelationTypes.Right;
            dargPeriodNumOfLives.RightOf.Offset = 250;
            dargPeriodNumOfLives.Above.Type = RelationTypes.Bot;
            dargPeriodNumOfLives.Above.Offset = 70;
            rgPeriodNumOfLives.DependencyArea = dargPeriodNumOfLives;
        }

        private static void TawuniaLegacySecondPageFields(DocClass result)
        {
            var OutPatient = new StaticTextRule("Outpatient", RuleBinding.Required);
            result.AddDataRule(OutPatient);
            OutPatient.TextToSearch = "OutPatient";
            OutPatient.SearchArea = new System.Windows.Rect(0, 0, 700, 700);

            var outpatientData = new CharacterStringRule("outpatientData", RuleBinding.Required);
            result.AddDataRule(outpatientData);
            outpatientData.DependencyRule = OutPatient;
            var opDataDA = new DependencyArea();
            opDataDA.Above.Type = RelationTypes.Bot;
            opDataDA.Above.Offset = 20;
            opDataDA.RightOf.Type = RelationTypes.Right;
            opDataDA.RightOf.Offset = 20;
            opDataDA.Below.Type = RelationTypes.Top;
            opDataDA.Below.Offset = -20;
            opDataDA.LeftOf.Type = RelationTypes.Right;
            opDataDA.LeftOf.Offset = 250;
            outpatientData.DependencyArea = opDataDA;
            outpatientData.TextToSearch = @"\d{1,4}";
            outpatientData.SearchArea = new System.Windows.Rect(0, 0, 1, 1);

            var outpatientAmt = new CharacterStringRule("outpatientAmount", RuleBinding.Required);
            result.AddDataRule(outpatientAmt);
            outpatientAmt.DependencyRule = outpatientData;
            var opAmtDA = new DependencyArea();
            opAmtDA.Above.Type = RelationTypes.Bot;
            opAmtDA.Above.Offset = 20;
            opAmtDA.RightOf.Type = RelationTypes.Right;
            opAmtDA.RightOf.Offset = 20;
            opAmtDA.Below.Type = RelationTypes.Top;
            opAmtDA.Below.Offset = -20;
            opAmtDA.LeftOf.Type = RelationTypes.Right;
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
            ipData.Above.Type = RelationTypes.Bot;
            ipData.Above.Offset = 20;
            ipData.RightOf.Type = RelationTypes.Right;
            ipData.RightOf.Offset = 20;
            ipData.Below.Type = RelationTypes.Top;
            ipData.Below.Offset = -20;
            ipData.LeftOf.Type = RelationTypes.Right;
            ipData.LeftOf.Offset = 250;
            inpatientData.DependencyArea = ipData;
            inpatientData.TextToSearch = @"\d{1,4}";
            inpatientData.SearchArea = new System.Windows.Rect(0, 0, 1, 1);

            var inpatientAmt = new CharacterStringRule("inpatientAmt", RuleBinding.Required);
            result.AddDataRule(inpatientAmt);
            inpatientAmt.DependencyRule = inpatientData;
            var ipAmt = new DependencyArea();
            ipAmt.Above.Type = RelationTypes.Bot;
            ipAmt.Above.Offset = 20;
            ipAmt.RightOf.Type = RelationTypes.Right;
            ipAmt.RightOf.Offset = 20;
            ipAmt.Below.Type = RelationTypes.Top;
            ipAmt.Below.Offset = -20;
            ipAmt.LeftOf.Type = RelationTypes.Right;
            ipAmt.LeftOf.Offset = 250;
            inpatientAmt.DependencyArea = ipAmt;
            inpatientAmt.TextToSearch = @"(\d+[\.,])+(\d{2})";
            inpatientAmt.SearchArea = new System.Windows.Rect(0, 0, 1, 1);
        }
        #endregion Tawunia
        #region BupaLegacy
        public static DocClass BupaLegacy()
        {
            var result = new DocClass("Bupa", 4);

            var stBupaArabia = new StaticTextRule("BupaArabia", RuleBinding.Required);
            stBupaArabia.TextToSearch = "Bupa Arabia for Cooperative Insurance";
            result.AddHeaderRule(stBupaArabia);
            stBupaArabia.SearchArea = new System.Windows.Rect(0, 0, 2000, 500);

            var stSignature = new StaticTextRule("SignatureNStamp", RuleBinding.Required);
            stSignature.TextToSearch = "Signature & Stamp";
            result.AddFooterRule(stSignature);
            stSignature.SearchArea = new System.Windows.Rect(0, 0, 1500, 3000);

            BupaLegacyAddHeadData(result);
            BupaLegacyAddTableData(result);

            return result;
        }
        private static void BupaLegacyAddHeadData(DocClass result)
        {
            var stInceptionDate = new StaticTextRule("stInceptionDate", RuleBinding.Required);
            stInceptionDate.TextToSearch = "Inception Date";
            stInceptionDate.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);
            result.AddDataRule(stInceptionDate);

            var stExpiryDate = new StaticTextRule("stExpiryDate", RuleBinding.Required);
            stExpiryDate.TextToSearch = "Expiry Date";
            stExpiryDate.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);
            result.AddDataRule(stExpiryDate);

            var csInceptionDate = new CharacterStringRule("InceptionDate", RuleBinding.Required);
            csInceptionDate.TextToSearch = @"[a-zA-Z]{3,4}\s?\d{1,2}[,\.]?\s?\d{4}";
            csInceptionDate.DependencyRule = stInceptionDate;
            result.AddDataRule(csInceptionDate);
            var dacsInceptionDate = new DependencyArea();
            dacsInceptionDate.Below.Type = RelationTypes.Top;
            dacsInceptionDate.Below.Offset = -5;
            dacsInceptionDate.Above.Type = RelationTypes.Bot;
            dacsInceptionDate.Above.Offset = 5;
            dacsInceptionDate.RightOf.Type = RelationTypes.Right;
            dacsInceptionDate.RightOf.Offset = 100;
            dacsInceptionDate.LeftOf.Type = RelationTypes.Right;
            dacsInceptionDate.LeftOf.Offset = 1100;
            csInceptionDate.DependencyArea = dacsInceptionDate;

            var csExpiryDate = new CharacterStringRule("ExpiryDate", RuleBinding.Required);
            csExpiryDate.TextToSearch = @"[a-zA-Z]{3,4}\s?\d{1,2}[,\.]?\s?\d{4}";
            csExpiryDate.DependencyRule = stExpiryDate;
            result.AddDataRule(csExpiryDate);
            var dacsExpiryDate = new DependencyArea();
            dacsExpiryDate.Below.Type = RelationTypes.Top;
            dacsExpiryDate.Below.Offset = -5;
            dacsExpiryDate.Above.Type = RelationTypes.Bot;
            dacsExpiryDate.Above.Offset = 5;
            dacsExpiryDate.RightOf.Type = RelationTypes.Right;
            dacsExpiryDate.RightOf.Offset = 100;
            dacsExpiryDate.LeftOf.Type = RelationTypes.Right;
            dacsExpiryDate.LeftOf.Offset = 1100;
            csExpiryDate.DependencyArea = dacsExpiryDate;
        }

        private static void BupaLegacyAddTableData(DocClass result)
        {
            BupaLegacyGet2YearPrior(result);

            BupaLegacyGetYearPrior(result);

            BupaLegacyGetLastYear(result);
        }

        private static void BupaLegacyGetLastYear(DocClass result)
        {
            var stLastYear = new StaticTextRule("LastYear", RuleBinding.Required);
            stLastYear.TextToSearch = "Policy Year";
            stLastYear.SearchArea = new System.Windows.Rect(0, 1500, 1000, 3000);
            result.AddDataRule(stLastYear);

            var rgcsMonthlyClaims = new RepeatingCSRule("LastYear_MonthlyClaim_0", RuleBinding.Required);
            rgcsMonthlyClaims.TextToSearch = @"\d{3,7}";
            rgcsMonthlyClaims.DependencyRule = stLastYear;
            rgcsMonthlyClaims.InterlineSpaces = 30;
            result.AddDataRule(rgcsMonthlyClaims);
            var dargMonthlyClaims = new DependencyArea();
            dargMonthlyClaims.Below.Type = RelationTypes.Bot;
            dargMonthlyClaims.Below.Offset = 10;
            dargMonthlyClaims.LeftOf.Type = RelationTypes.Right;
            dargMonthlyClaims.LeftOf.Offset = 20;
            dargMonthlyClaims.RightOf.Type = RelationTypes.Left;
            //dargMonthlyClaims.RightOf.Offset = 250;
            dargMonthlyClaims.Above.Type = RelationTypes.Bot;
            dargMonthlyClaims.Above.Offset = 70;
            rgcsMonthlyClaims.DependencyArea = dargMonthlyClaims;

            var rgcsNumberOfLives = new RepeatingCSRule("LastYear_NumberOfLives_0", RuleBinding.Required);
            rgcsNumberOfLives.TextToSearch = @"[\da-zA-Z]{1,3}";
            rgcsNumberOfLives.InterlineSpaces = 30;
            rgcsNumberOfLives.DependencyRule = stLastYear;
            result.AddDataRule(rgcsNumberOfLives);
            var dargNumberOfLives = new DependencyArea();
            dargNumberOfLives.Below.Type = RelationTypes.Bot;
            dargNumberOfLives.Below.Offset = 10;
            dargNumberOfLives.LeftOf.Type = RelationTypes.Right;
            dargNumberOfLives.LeftOf.Offset = 250;
            dargNumberOfLives.RightOf.Type = RelationTypes.Right;
            dargNumberOfLives.RightOf.Offset = 150;
            dargNumberOfLives.Above.Type = RelationTypes.Bot;
            dargNumberOfLives.Above.Offset = 70;
            rgcsNumberOfLives.DependencyArea = dargNumberOfLives;
        }

        private static void BupaLegacyGetYearPrior(DocClass result)
        {
            var stYearPrior = new StaticTextRule("YearPrior", RuleBinding.Required);
            stYearPrior.TextToSearch = "Policy Year";
            stYearPrior.SearchArea = new System.Windows.Rect(0, 900, 1000, 2000);
            result.AddDataRule(stYearPrior);

            var rgcsMonthlyClaims = new RepeatingCSRule("YearPrior_MonthlyClaim_0", RuleBinding.Required);
            rgcsMonthlyClaims.TextToSearch = @"\d{3,7}";
            rgcsMonthlyClaims.DependencyRule = stYearPrior;
            rgcsMonthlyClaims.InterlineSpaces = 30;
            result.AddDataRule(rgcsMonthlyClaims);
            var dargMonthlyClaims = new DependencyArea();
            dargMonthlyClaims.Below.Type = RelationTypes.Bot;
            dargMonthlyClaims.Below.Offset = 10;
            dargMonthlyClaims.LeftOf.Type = RelationTypes.Right;
            dargMonthlyClaims.LeftOf.Offset = 20;
            dargMonthlyClaims.RightOf.Type = RelationTypes.Left;
            //dargMonthlyClaims.RightOf.Offset = 250;
            dargMonthlyClaims.Above.Type = RelationTypes.Bot;
            dargMonthlyClaims.Above.Offset = 70;
            rgcsMonthlyClaims.DependencyArea = dargMonthlyClaims;

            var rgcsNumberOfLives = new RepeatingCSRule("YearPrior_NumberOfLives_0", RuleBinding.Required);
            rgcsNumberOfLives.TextToSearch = @"[\da-zA-Z]{1,3}";
            rgcsNumberOfLives.InterlineSpaces = 30;
            rgcsNumberOfLives.DependencyRule = stYearPrior;
            result.AddDataRule(rgcsNumberOfLives);
            var dargNumberOfLives = new DependencyArea();
            dargNumberOfLives.Below.Type = RelationTypes.Bot;
            dargNumberOfLives.Below.Offset = 10;
            dargNumberOfLives.LeftOf.Type = RelationTypes.Right;
            dargNumberOfLives.LeftOf.Offset = 250;
            dargNumberOfLives.RightOf.Type = RelationTypes.Right;
            dargNumberOfLives.RightOf.Offset = 150;
            dargNumberOfLives.Above.Type = RelationTypes.Bot;
            dargNumberOfLives.Above.Offset = 70;
            rgcsNumberOfLives.DependencyArea = dargNumberOfLives;
        }

        private static void BupaLegacyGet2YearPrior(DocClass result)
        {
            var st2YPrior = new StaticTextRule("2YearsPrior", RuleBinding.Required);
            st2YPrior.TextToSearch = "Policy Year";
            st2YPrior.SearchArea = new System.Windows.Rect(0, 0, 1000, 1000);
            result.AddDataRule(st2YPrior);

            var rgcsMonthlyClaims = new RepeatingCSRule("2YPrior_MonthlyClaim_0", RuleBinding.Required);
            rgcsMonthlyClaims.TextToSearch = @"\d{3,7}";
            rgcsMonthlyClaims.DependencyRule = st2YPrior;
            rgcsMonthlyClaims.InterlineSpaces = 30;
            result.AddDataRule(rgcsMonthlyClaims);
            var dargMonthlyClaims = new DependencyArea();
            dargMonthlyClaims.Below.Type = RelationTypes.Bot;
            dargMonthlyClaims.Below.Offset = 10;
            dargMonthlyClaims.LeftOf.Type = RelationTypes.Right;
            dargMonthlyClaims.LeftOf.Offset = 50;
            dargMonthlyClaims.RightOf.Type = RelationTypes.Left;
            //dargMonthlyClaims.RightOf.Offset = 250;
            dargMonthlyClaims.Above.Type = RelationTypes.Bot;
            dargMonthlyClaims.Above.Offset = 70;
            rgcsMonthlyClaims.DependencyArea = dargMonthlyClaims;

            var rgcsNumberOfLives = new RepeatingCSRule("2YPrior_NumberOfLives_0", RuleBinding.Required);
            rgcsNumberOfLives.TextToSearch = @"[\da-zA-Z]{1,3}";
            rgcsNumberOfLives.DependencyRule = st2YPrior;
            rgcsNumberOfLives.InterlineSpaces = 30;
            result.AddDataRule(rgcsNumberOfLives);
            var dargNumberOfLives = new DependencyArea();
            dargNumberOfLives.Below.Type = RelationTypes.Bot;
            dargNumberOfLives.Below.Offset = 10;
            dargNumberOfLives.LeftOf.Type = RelationTypes.Right;
            dargNumberOfLives.LeftOf.Offset = 320;
            dargNumberOfLives.RightOf.Type = RelationTypes.Right;
            dargNumberOfLives.RightOf.Offset = 200;
            dargNumberOfLives.Above.Type = RelationTypes.Bot;
            dargNumberOfLives.Above.Offset = 70;
            rgcsNumberOfLives.DependencyArea = dargNumberOfLives;
        }
        #endregion BupaLegacy
        #endregion Legacy

        public static DocClass Bupa()
        {
            var result = new DocClass("BupaTest", 4);
            var stBupaArab = new StaticTextRule("BupaArab", RuleBinding.Required);
            var firstConstr = new SearchConstraint().Page().Above().YCenter();
            stBupaArab.TextToSearch = "Bupa Arabia";
            //var firstConstr = (new SearchConstraint()).Page().Above.XCenter;
            stBupaArab.SearchConstraints.Add(firstConstr);
            result.AddHeaderRule(stBupaArab);
            return result;
        }
        public void xxx()
        {

        }
    }
}
