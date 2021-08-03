using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using experian.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace experian.Controllers
{
    public class ClientsController : Controller
    {
        
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public IActionResult Index(List<List<string>> clientsData)
        {
            return View(clientsData);
        }
        bool error = false;
        bool success = false;

        int indexOfExisting_Customer_Flag = 0;
        int indexOfNumber_of_Dependants = 0;
        int indexOfOccupation_Code = 0;
        int indexOfResidential_Status = 0;
        int indexOfTime_in_Employment = 0;
        int indexOfBureau_Score = 0;
        int indexOfSP_ER_Reference = 0;
        int indexOfSP_Number_Of_Searches_L6M = 0;
        int indexOfSP_Number_of_CCJs = 0;

        bool isMissingExisting_Customer_Flag = true;
        bool isMissingNumber_of_Dependants = true;
        bool isMissingOccupation_Code = true;
        bool isMissingResidential_Status = true;
        bool isMissingTime_in_Employment = true;
        bool isMissingBureau_Score = true;
        bool isMissingSP_ER_Reference = true;
        bool isMissingSP_Number_Of_Searches_L6M = true;
        bool isMissingSP_Number_of_CCJs = true;

        [HttpPost]
        [Authorize]
        public IActionResult Index(IFormFile file)
        {
            if (file == null)
            {
                error = true;
                ViewBag.Error = error;
                ViewBag.ErrorText = "Select a CSV file to upload!";
                return View("UploadError");
            }
            #region Upload CSV
            string fileName = $"{Directory.GetCurrentDirectory()}{@"\Files"}" + "\\" + file.FileName;
            string extension = fileName.Substring(fileName.Length - 3, 3);
            if (extension != "csv")
            {
                error = true;
                ViewBag.Error = error;
                ViewBag.ErrorText = "The file uploaded is not in the correct format, please upload a csv file";
                return View("UploadError");
            }
            if (System.IO.File.Exists(fileName))
            {
                error = true;
                ViewBag.Error = error;
                ViewBag.ErrorText = "The file you are trying to upload already exists on the file system!";
                return View("UploadError");
            }
            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            #endregion

            var records = this.GetClientsDataList(file.FileName);
            if (success)
            {
                return View("Success");
            }
            return Index(records);
        }

        private List<List<string>> GetClientsDataList(string fileName)
        {
            string Existing_Customer_Flag = "Existing_Customer_Flag";
            string Number_of_Dependants = "Number_of_Dependants";
            string Occupation_Code = "Occupation_Code";
            string Residential_Status = "Residential_Status";
            string Time_in_Employment = "Time_in_Employment";
            string Bureau_Score = "Bureau_Score";
            string SP_ER_Reference = "SP_ER_Reference";
            string SP_Number_Of_Searches_L6M = "SP_Number_Of_Searches_L6M";
            string SP_Number_of_CCJs = "SP_Number_of_CCJs";

            List<List<string>> records = new List<List<string>>();
            #region Read CSV
            var path = $"{Directory.GetCurrentDirectory()}{@"\Files"}";
            using (TextFieldParser csv = new TextFieldParser(path + "\\" + fileName))
            {
                csv.SetDelimiters(new string[] { "," });
                string columnNames = csv.ReadLine();

                string[] columns = columnNames.Split(";");

                for (int i = 0; i < columns.Length; i++)
                {
                    if (columns[i].Equals(Existing_Customer_Flag))
                    {
                        indexOfExisting_Customer_Flag = i;
                        isMissingExisting_Customer_Flag = false;
                        continue;
                    }
                    if (columns[i].Equals(Number_of_Dependants))
                    {
                        indexOfNumber_of_Dependants = i;
                        isMissingNumber_of_Dependants = false;
                        continue;
                    }
                    if (columns[i].Equals(Occupation_Code))
                    {
                        indexOfOccupation_Code = i;
                        isMissingOccupation_Code = false;
                        continue;
                    }
                    if (columns[i].Equals(Residential_Status))
                    {
                        indexOfResidential_Status = i;
                        isMissingResidential_Status = false;
                        continue;
                    }
                    if (columns[i].Equals(Time_in_Employment))
                    {
                        indexOfTime_in_Employment = i;
                        isMissingTime_in_Employment = false;
                        continue;
                    }
                    if (columns[i].Equals(Bureau_Score))
                    {
                        indexOfBureau_Score = i;
                        isMissingBureau_Score = false;
                        continue;
                    }
                    if (columns[i].Equals(SP_ER_Reference))
                    {
                        indexOfSP_ER_Reference = i;
                        isMissingSP_ER_Reference = false;
                        continue;
                    }
                    if (columns[i].Equals(SP_Number_Of_Searches_L6M))
                    {
                        indexOfSP_Number_Of_Searches_L6M = i;
                        isMissingSP_Number_Of_Searches_L6M = false;
                        continue;
                    }
                    if (columns[i].Equals(SP_Number_of_CCJs))
                    {
                        indexOfSP_Number_of_CCJs = i;
                        isMissingSP_Number_of_CCJs = false;
                        continue;
                    }
                }

                while (!csv.EndOfData)
                {
                    try
                    {
                        string[] fields = csv.ReadFields();
                        List<string> data = fields[0].Split(";").ToList();
                        records.Add(data);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    
                }
                Console.WriteLine();
            }
            //CalculateScore(records);
            addToFile(CalculateScore(records), path + "\\result" + fileName);
            success = true;
            return records;
            #endregion
            
        }
        private Dictionary<long,string> CalculateScore(List<List<string>> records) 
        {
            Dictionary<long, string> finalResult = new Dictionary<long, string>();
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
            long lNumber;
            bool isExistingWrongColumn = false;
            int j = -1;


            if (isMissingExisting_Customer_Flag)
            {
                finalResult.Add(j--, "Wrong column name of Existing_Customer_Flag! It should be Existing_Customer_Flag");
                isExistingWrongColumn = true;
            }
            if (isMissingNumber_of_Dependants)
            {
                finalResult.Add(j--, "Wrong column name of Number_of_Dependants! It should be Number_of_Dependants");
                isExistingWrongColumn = true;
            }
            if (isMissingOccupation_Code)
            {
                finalResult.Add(j--, "Wrong column name of Occupation_Code! It should be Occupation_Code");
            }
            if (isMissingResidential_Status)
            {
                finalResult.Add(j--, "Wrong column name of Residential_Status! It should be Residential_Status");
                isExistingWrongColumn = true;
            }
            if (isMissingTime_in_Employment)
            {
                finalResult.Add(j--, "Wrong column name of Time_in_Employment! It should be Time_in_Employment");
                isExistingWrongColumn = true;
            }
            if (isMissingBureau_Score)
            {
                finalResult.Add(j--, "Wrong column name of Bureau_Score! It should be Bureau_Score");
                isExistingWrongColumn = true;
            }
            if (isMissingSP_ER_Reference)
            {
                finalResult.Add(j--, "Wrong column name of SP_ER_Reference! It should be SP_ER_Reference");
                isExistingWrongColumn = true;
            }
            if (isMissingSP_Number_Of_Searches_L6M)
            {
                finalResult.Add(j--, "Wrong column name of SP_Number_Of_Searches_L6M! It should be SP_Number_Of_Searches_L6M");
                isExistingWrongColumn = true;
            }
            if (isMissingSP_Number_of_CCJs)
            {
                finalResult.Add(j--, "Wrong column name of SP_Number_of_CCJs! It should be SP_Number_of_CCJs");
                isExistingWrongColumn = true;
            }

            if (isExistingWrongColumn)
                return finalResult;

            for (int i = 0; i < records.Count; i++)
                {
                    if (records[i][indexOfExisting_Customer_Flag] == "")
                    {
                        if (long.TryParse(records[i][0], out lNumber))
                        {
                            finalResult.Add(long.Parse(records[i][0]), "Invalid Existing Customer Flag!");
                            continue;
                        }   
                        else
                        {
                            finalResult.Add(i, "Invalid Existing Customer Flag!");
                            continue;
                        }
                        
                    }
                    if (records[i][indexOfExisting_Customer_Flag] == "N")
                    {
                        existing_Customer_Flag_value = 0.1532;
                    }
                    else if (records[i][indexOfExisting_Customer_Flag] == "Y")
                    {
                        existing_Customer_Flag_value = 0.2840;
                    }

                    if (records[i][indexOfNumber_of_Dependants] == "" || !(int.TryParse(records[i][15], out number)))
                    {
                        if (long.TryParse(records[i][0], out lNumber))
                        {
                            finalResult.Add(long.Parse(records[i][0]),              "Invalid Number of Dependants!");
                            continue;
                        }
                        else
                        {
                            finalResult.Add(i, "Invalid Number of                   Dependants!");
                            continue;
                        }
                    }
                    if (int.Parse(records[i][indexOfNumber_of_Dependants]) < 3)
                    {
                        number_of_Dependants_value = 0.2950;
                    }
                    else if (int.Parse(records[i][indexOfNumber_of_Dependants]) >= 3)
                    {
                        number_of_Dependants_value = 0.1422;
                    }

                    if (records[i][indexOfOccupation_Code] == "")
                    {
                        if (long.TryParse(records[i][0], out lNumber))
                        {
                            finalResult.Add(long.Parse(records[i][0]),              "Invalid Occupation Code!");
                            continue;
                        }
                        else
                        {
                            finalResult.Add(i, "Invalid Occupation                  Code!");
                            continue;
                        }
                    }
                    if (records[i][indexOfOccupation_Code] == "M" || records[i][indexOfOccupation_Code] == "B")
                    {
                        occupation_Code_value = 0.1570;
                    }
                    else if (records[i][indexOfOccupation_Code] == "P" || records[i][indexOfOccupation_Code] == "O")
                    {
                        occupation_Code_value = 0.2802;
                    }

                    if (records[i][indexOfResidential_Status] == "")
                    {
                        if (long.TryParse(records[i][0], out lNumber))
                        {
                            finalResult.Add(long.Parse(records[i][0]),              "Invalid Residential Status!");
                            continue;
                        }
                        else
                        {
                            finalResult.Add(i, "Invalid Residential                 Status!");
                            continue;
                        }
                    }
                    if (records[i][indexOfResidential_Status] == "H")
                    {
                        residential_Status_value = 0.2305;
                    }
                    else if (records[i][indexOfResidential_Status] == "L" || records[i][indexOfResidential_Status] == "O" || records[i][indexOfResidential_Status] == "T")
                    {
                        residential_Status_value = 0.2067;
                    }

                    if (records[i][indexOfTime_in_Employment] == "" || !(int.TryParse(records[i][indexOfTime_in_Employment], out number)))
                    {
                        if (long.TryParse(records[i][0], out lNumber))
                        {
                            finalResult.Add(long.Parse(records[i][0]),              "Invalid Time in Employment!");
                            continue;
                        }
                        else
                        {
                            finalResult.Add(i, "Invalid Time in                     Employment!");
                            continue;
                        }
                    }
                    if (int.Parse(records[i][indexOfTime_in_Employment]) < 500)
                    {
                        time_in_Employment_value = 0.1369;
                    }
                    else if (int.Parse(records[i][indexOfTime_in_Employment]) >= 500)
                    {
                        time_in_Employment_value = 0.3003;
                    }

                if (records[i][indexOfBureau_Score] == "" || !(int.TryParse(records[i][indexOfBureau_Score], out number)))
                {
                    if (long.TryParse(records[i][0], out lNumber))
                    {
                        finalResult.Add(long.Parse(records[i][0]),              "Invalid Bureau Score!");
                        continue;
                    }
                    else
                    {
                        finalResult.Add(i, "Invalid Bureau Score!");
                        continue;
                    }
                }
                if (int.Parse(records[i][indexOfBureau_Score]) < 746)
                {
                    bureau_Score_value = -1.5400;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) < 803)
                {
                    bureau_Score_value = -1.1212;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) < 844)
                {
                    bureau_Score_value = -0.7227;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) < 859)
                {
                    bureau_Score_value = -0.6100;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) < 875)
                {
                    bureau_Score_value = -0.3877;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) < 902)
                {
                    bureau_Score_value = -0.2483;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) < 922)
                {
                    bureau_Score_value = -0.2353;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) < 950)
                {
                    bureau_Score_value = 0.2685;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) < 972)
                {
                    bureau_Score_value = 0.4053;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) < 990)
                {
                    bureau_Score_value = 0.9314;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) < 1012)
                {
                    bureau_Score_value = 0.9770;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) < 1025)
                {
                    bureau_Score_value = 1.0831;
                }
                else if (int.Parse(records[i][indexOfBureau_Score]) >= 1025)
                {
                    bureau_Score_value = 1.6371;
                }

                if (records[i][indexOfSP_ER_Reference] == "" || !(int.TryParse(records[i][indexOfSP_ER_Reference], out number)))
                {
                    if (long.TryParse(records[i][0], out lNumber))
                    {
                        finalResult.Add(long.Parse(records[i][0]),              "Invalid SP ER Reference!");
                        continue;
                    }
                    else
                    {
                        finalResult.Add(i, "Invalid SP ER Reference!");
                        continue;
                    }
                }
                if (int.Parse(records[i][indexOfSP_ER_Reference]) <= 2)
                {
                    sp_ER_Reference_value = 0.1647;
                }
                else if (int.Parse(records[i][indexOfSP_ER_Reference]) >= 3)
                {
                    sp_ER_Reference_value = 0.2725;
                }

                if (records[i][indexOfSP_Number_Of_Searches_L6M] == "" || !(int.TryParse(records[i][indexOfSP_Number_Of_Searches_L6M], out number)))
                {
                    if (long.TryParse(records[i][0], out lNumber))
                    {
                        finalResult.Add(long.Parse(records[i][0]),              "Invalid SP Number of Searches L6M!");
                        continue;
                    }
                    else
                    {
                        finalResult.Add(i, "Invalid SP Number of Searches       L6M!");
                        continue;
                    }
                }
                if (int.Parse(records[i][indexOfSP_Number_Of_Searches_L6M]) < 3)
                {
                    sp_Number_Of_Searches_L6M_value = 0.3160;
                }
                else if (int.Parse(records[i][indexOfSP_Number_Of_Searches_L6M]) >= 3)
                {
                    sp_Number_Of_Searches_L6M_value = 0.1212;
                }

                if (records[i][indexOfSP_Number_of_CCJs] == "" || !(int.TryParse(records[i][indexOfSP_Number_of_CCJs], out number)))
                {
                    if (long.TryParse(records[i][0], out lNumber))
                    {
                        finalResult.Add(long.Parse(records[i][0]),              "Invalid SP Number of CCJs!");
                        continue;
                    }
                    else
                    {
                        finalResult.Add(i, "Invalid SP Number of CCJs!");
                        continue;
                    }
                }
                if (int.Parse(records[i][indexOfSP_Number_of_CCJs]) < 1)
                {
                    sp_Number_of_CCJs_value = 0.7142;
                }
                else if (int.Parse(records[i][indexOfSP_Number_of_CCJs]) < 6)
                {
                    sp_Number_of_CCJs_value = 0.5015;
                }
                else if (int.Parse(records[i][indexOfSP_Number_of_CCJs]) >= 6)
                {
                    sp_Number_of_CCJs_value = -0.7785;

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
                if(long.TryParse(records[i][0], out lNumber))
                {
                    finalResult.Add(long.Parse(records[i][0]), "-           Score: " + Math.Round(score, 2) + ", Result: " +        result);
                }
                else
                {
                    finalResult.Add(i, "- Score: " + Math.Round(score, 2) + ", Result: " + result);
                }

            }
            
            Console.WriteLine();
            return finalResult;
        }
        private void addToFile(Dictionary<long, string> result, string filename) 
        {
            using (FileStream fileStream = System.IO.File.Create(filename))
            {

            }
            using (StreamWriter file = new StreamWriter(filename)) 
            {
                foreach (var entry in result)
                {
                    file.WriteLine("[{0} {1}]", entry.Key, entry.Value);
                }
            }
            
        }
    }
}
