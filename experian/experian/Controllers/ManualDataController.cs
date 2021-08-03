using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace experian.Controllers
{
    public class ManualDataController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Index(IFormCollection form)
        {
            //string Account_Number = form["Account_Number"];
            //string Account_Type = form["Account_Type"];
            //string Final_Decision = form["Final_Decision"];
            //string Current_Delinquency_status = form["Current_Delinquency_status"];
            //string Application_Date = form["Application_Date"];
            //string Application_Score = form["Application_Score"];
            //string Cheque_Card_Flag = form["Cheque_Card_Flag"];
            string Existing_Customer_Flag = form["Existing_Customer_Flag"];
            string Gross_Annual_Income = form["Gross_Annual_Income"];
            string Home_Telephone_Number = form["Home_Telephone_Number"];
            string Insurance_Required = form["Insurance_Required"];
            string Loan_Amount = form["Loan_Amount"];
            string Loan_Payment_Frequency = form["Loan_Payment_Frequency"];
            string Loan_Payment_Method = form["Loan_Payment_Method"];
            string Marital_Status = form["Marital_Status"];
            string Number_of_Dependants = form["Number_of_Dependants"];
            //string Number_of_Payments = form["Number_of_Payments"];
            string Occupation_Code = form["Occupation_Code"];
            //string Promotion_Type = form["Promotion_Type"];
            string Residential_Status = form["Residential_Status"];
            //string Time_at_Address = form["Time_at_Address"];
            string Time_in_Employment = form["Time_in_Employment"];
            //string Time_with_Bank = form["Time_with_Bank"];
            //string Weight_Factor = form["Weight_Factor"];
            //string GB_Flag = form["GB_Flag"];
            //string Age_of_Applicant = form["Age_of_Applicant"];
            //string Application_Month = form["Application_Month"];
            string Bureau_Score = form["Bureau_Score"];
            string SP_ER_Reference = form["SP_ER_Reference"];
            string SP_Number_Of_Searches_L6M = form["SP_Number_Of_Searches_L6M"];
            string SP_Number_of_CCJs = form["SP_Number_of_CCJs"];
            //string loan_to_income = form["loan_to_income"];
            //string split = form["split"];

            Console.WriteLine();

            double existing_Customer_Flag_value = 0;
            double number_of_Dependants_value = 0;
            double occupation_Code_value = 0;
            double residential_Status_value = 0;
            double time_in_Employment_value = 0;
            double bureau_Score_value = 0;
            double sp_ER_Reference_value = 0;
            double sp_Number_Of_Searches_L6M_value = 0;
            double sp_Number_of_CCJs_value = 0;
            double value = 0;
            double score = 0;
            string result = "";

            int number;
            bool isWrong = false;

            if (Existing_Customer_Flag == "N")
            {
                existing_Customer_Flag_value = 0.1532;
            }
            else if (Existing_Customer_Flag == "Y")
            {
                existing_Customer_Flag_value = 0.2840;
            }

            if (int.TryParse(Number_of_Dependants, out number))
            {
                if (int.Parse(Number_of_Dependants) < 3)
                {
                    number_of_Dependants_value = 0.2950;
                }
                else if (int.Parse(Number_of_Dependants) >= 3)
                {
                    number_of_Dependants_value = 0.1422;
                }
            }
            else
            {
                isWrong = true;
            }
            
            if (Occupation_Code == "M" || Occupation_Code == "B")
            {
                occupation_Code_value = 0.1570;
            }
            else if (Occupation_Code == "P" || Occupation_Code == "O")
            {
                occupation_Code_value = 0.2802;
            }

            if (Residential_Status == "H")
            {
                residential_Status_value = 0.2305;
            }
            else if (Residential_Status == "L" || Residential_Status == "O" || Residential_Status == "T")
            {
                residential_Status_value = 0.2067;
            }

            if (int.TryParse(Time_in_Employment, out number))
            {
                if (int.Parse(Time_in_Employment) < 500)
                {
                    time_in_Employment_value = 0.1369;
                }
                else if (int.Parse(Time_in_Employment) >= 500)
                {
                    time_in_Employment_value = 0.3003;
                }
            }
            else
            {
                isWrong = true;
            }

            if (int.TryParse(Bureau_Score, out number))
            {
                if (int.Parse(Bureau_Score) < 746)
                {
                    bureau_Score_value = -1.5400;
                }
                else if (int.Parse(Bureau_Score) < 803)
                {
                    bureau_Score_value = -1.1212;
                }
                else if (int.Parse(Bureau_Score) < 844)
                {
                    bureau_Score_value = -0.7227;
                }
                else if (int.Parse(Bureau_Score) < 859)
                {
                    bureau_Score_value = -0.6100;
                }
                else if (int.Parse(Bureau_Score) < 875)
                {
                    bureau_Score_value = -0.3877;
                }
                else if (int.Parse(Bureau_Score) < 902)
                {
                    bureau_Score_value = -0.2483;
                }
                else if (int.Parse(Bureau_Score) < 922)
                {
                    bureau_Score_value = -0.2353;
                }
                else if (int.Parse(Bureau_Score) < 950)
                {
                    bureau_Score_value = 0.2685;
                }
                else if (int.Parse(Bureau_Score) < 972)
                {
                    bureau_Score_value = 0.4053;
                }
                else if (int.Parse(Bureau_Score) < 990)
                {
                    bureau_Score_value = 0.9314;
                }
                else if (int.Parse(Bureau_Score) < 1012)
                {
                    bureau_Score_value = 0.9770;
                }
                else if (int.Parse(Bureau_Score) < 1025)
                {
                    bureau_Score_value = 1.0831;
                }
                else if (int.Parse(Bureau_Score) >= 1025)
                {
                    bureau_Score_value = 1.6371;
                }
            }
            else
            {
                isWrong = true;
            }

            if (int.TryParse(SP_ER_Reference, out number))
            {
                if (int.Parse(SP_ER_Reference) <= 2)
                {
                    sp_ER_Reference_value = 0.1647;
                }
                else if (int.Parse(SP_ER_Reference) >= 3)
                {
                    sp_ER_Reference_value = 0.2725;
                }
            }
            else
            {
                isWrong = true;
            }

            if (int.TryParse(SP_Number_Of_Searches_L6M, out number))
            {
                if (int.Parse(SP_Number_Of_Searches_L6M) < 3)
                {
                    sp_Number_Of_Searches_L6M_value = 0.3160;
                }
                else if (int.Parse(SP_Number_Of_Searches_L6M) >= 3)
                {
                    sp_Number_Of_Searches_L6M_value = 0.1212;
                }
            }
            else
            {
                isWrong = true;
            }

            if (int.TryParse(SP_Number_of_CCJs, out number))
            {
                if (int.Parse(SP_Number_of_CCJs) < 1)
                {
                    sp_Number_of_CCJs_value = 0.7142;
                }
                else if (int.Parse(SP_Number_of_CCJs) < 6)
                {
                    sp_Number_of_CCJs_value = 0.5015;
                }
                else if (int.Parse(SP_Number_of_CCJs) >= 6)
                {
                    sp_Number_of_CCJs_value = -0.7785;

                }
            }
            else
            {
                isWrong = true;
            }

            value = existing_Customer_Flag_value + number_of_Dependants_value + occupation_Code_value
                    + residential_Status_value + time_in_Employment_value + bureau_Score_value + sp_ER_Reference_value + sp_Number_of_CCJs_value
                    + sp_Number_Of_Searches_L6M_value;
            score = (0.8275 + value) * 100;

            if (score > 300)
            {
                result = "good";
            }
            else if (score < 250)
            {
                result = "bad";
            }
            else if (score >= 250 && score <= 300)
            {
                result = "manually review";
            }

            Console.WriteLine();
            if (isWrong)
            {
                return View("Error");
            }
            ViewBag.Result = "Your credit score is " + Math.Round(score, 2) + ", " + result;
            return View("Success");


        }

    }
}
