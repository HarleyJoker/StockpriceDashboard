
# **Stock Price Dashboard Web API**

## **Project Overview**
The **Stock Price Dashboard Web API** is a RESTful API that provides historical stock price data (open, high, low, close, volume) for various companies. It fetches data from Alpha Vantage API and delivers it in a structured JSON format to frontend consumers.

## **Technologies Used**
- **Backend Framework**: ASP.NET Core 8.0
- **Language**: C#
- **API Integration**: Alpha Vantage API via RapidAPI
- **Documentation**: Swagger
- **Caching**: In-Memory Caching
- **Testing Tools**: Postman, Swagger UI

### **Prerequisites**
1. Install **.NET 8.0 SDK**
2. Obtain an API key from [Alpha Vantage via RapidAPI](https://rapidapi.com/).

### **Steps to Run**
1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd StockpriceDashboard

### **Configure API key in appsettings.json**
{
  "AlphaVantage": {
    "BaseUrl": "https://alpha-vantage.p.rapidapi.com/query",
    "ApiKey": "YOUR_API_KEY"
  }
}

###  **Restore dependencies and run the project**
dotnet restore
dotnet run


### **Access Swagger for API testing and documentation**
http://localhost:<port>/swagger

### **Features**
Fetch daily stock prices for companies like MSFT, AAPL, NFLX, FB, AMZN.
Includes caching for performance optimization.
