# ExperianWorkShopMyCS
Web App for evaluating Credit Score
A Website with authentication 
Two roles - admin and user.
User role can upload CSV  file and enter manual data.
Admin can do the same as user as well as adding, deleting, editing and listing users.
The main functionality is calculating credit score based on data.
It uses analytics logic for the calculation.
For manual data gives the result of the single record in format - score - good, bad or needs manual review by human
For batch processing - upload a CSV file with multiple rows. Each row is a single record (bank's customer) with the details needed to calculate the credit score. As a result it generates new file with the id of the record and the result for each record.
