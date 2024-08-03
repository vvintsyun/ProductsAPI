The requirement for task:
Create a .NET Core solution to contain an HTTP endpoint that:
1. Accepts a GET request with three optional query parameters to filter products or
highlight some words (separated by commas in the query parameter) in their description:
Example:
/filter?minprice=5&maxprice=20&size=medium&highlight=green,blue
2. Reads the list of all products from the URL (think of this as the database). 
Example of data is duplicated in file Data/data.json
3. Design the endpoint response so that it contains (in JSON format):
    a. All products if the request has no parameters
    b. A filtered subset of products if the request has filter parameters
    c. A filter object to contain:
        i. The minimum and the maximum price of all products in the source URL
        ii. An array of strings to contain all sizes of all products in the source URL
        iii. A string array of size ten to contain the most common words in all of the
        product descriptions, excluding the most common five

    d. Add HTML tags to returned product descriptions in order to highlight the words
    provided in the highlight parameter.
    Example: “These trousers make a perfect pair with <em>green</em> or
    <em>blue</em> shirts.”