# Facebook Automation Solution

This repository provides a comprehensive Facebook automation solution using C#. It supports multiple automation tasks such as logging in, sending friend requests, posting comments, reacting to posts, joining Facebook groups, and fetching data from Facebook.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Requirements](#requirements)
- [Setup & Installation](#setup--installation)
- [Usage](#usage)
  - [Logging in to Facebook](#logging-in-to-facebook)
  - [Performing User Actions](#performing-user-actions)
  - [Fetching Facebook Data](#fetching-facebook-data)
- [Configuration](#configuration)
- [Services & Dependencies](#services--dependencies)
- [Contributing](#contributing)
- [License](#license)

## Overview

The Facebook Automation solution is designed to automate various tasks on Facebook. This includes logging into Facebook, interacting with user posts, sending friend requests, joining groups, and even fetching data for analysis. The solution makes use of modern C# practices and leverages dependency injection to ensure easy extensibility and testing.

### Key Functionalities:

- **Login Automation**: Automates the process of logging into Facebook, including proxy support.
- **User Actions**: Includes sending bulk friend requests, following, poking users, and commenting on posts.
- **Group Actions**: Supports sending join requests to Facebook groups.
- **Post Reactions**: Automates reactions (e.g., "Like", "Love", "Care", etc.) on posts.
- **Fetch Data**: Fetches Facebook user data, page data, and post comments.

## Features

- **Facebook Login**: Use provided credentials to login to Facebook with optional proxy support.
- **Facebook Actions**: Automate actions like sending friend requests, poking, or following users.
- **Group Management**: Join Facebook groups programmatically.
- **React to Posts**: React to Facebook posts using various reaction types.
- **Comment on Posts**: Post comments on user posts or page posts.
- **Data Fetching**: Retrieve user data, page data, and comments from Facebook posts using GraphQL API.

## Requirements

- **.NET 6.0** or later (for building and running the application)
- **C# 10.0** or later
- **Visual Studio** (or any C# IDE of choice)
- **Facebook credentials** (for login)
- **Proxy settings** (optional, for using proxies during automation)
- **Facebook API access** (for GraphQL requests)

## Setup & Installation

1. **Clone the repository**:

   ```bash
   git clone https://github.com/yourusername/facebook-automation.git
   cd facebook-automation
   ```

2. **Install dependencies**:

   If using Visual Studio, open the solution file (`FacebookAutomation.sln`). It should automatically restore all required dependencies.

   Alternatively, if using the command line, run:

   ```bash
   dotnet restore
   ```

3. **Configure Application**:

   - Open `appsettings.json` and configure your Facebook credentials and proxy settings (if applicable).
   - Make sure to replace any placeholder data with your actual credentials and configurations.

   Example:

   ```json
   {
     "FacebookAuth": {
       "Email": "your_email@example.com",
       "Password": "your_password",
       "ProxySettings": {
         "IpAddress": "your.proxy.ip",
         "Port": "proxy_port",
         "Username": "proxy_username",
         "Password": "proxy_password"
       }
     },
     "ApiUrl": "https://www.facebook.com/api/graphql/"
   }
   ```

4. **Run the application**:

   You can now build and run the application. For Visual Studio users, simply press **F5**. For command line:

   ```bash
   dotnet run
   ```

## Usage

### Logging in to Facebook

To log in to Facebook, instantiate a `FacebookAuthModel` with your credentials and proxy settings (if necessary), and call the `Login` method:

```csharp
var facebookAuth = new FacebookAuthModel
{
    Email = "your_email@example.com",
    Password = "your_password",
    ProxySettings = new ProxySettings
    {
        IpAddress = "proxy_ip",
        Port = "proxy_port",
        Username = "proxy_username",
        Password = "proxy_password"
    }
};

var facebookLoginAutomation = new FacebookLoginAutomation();
var cookies = facebookLoginAutomation.Login(facebookAuth);
```

### Performing User Actions

Once logged in, you can perform various actions such as sending friend requests, poking, or following users:

```csharp
var userActionsService = new FacebookUserActionsService();
await userActionsService.SendBulkFriendRequests(new List<string> { "user_id" });
await userActionsService.PokeUser("user_id");
await userActionsService.FollowUser("user_id");
```

### Fetching Facebook Data

You can fetch user data, page data, or comments from Facebook posts using the `FacebookDataFetcher`:

```csharp
await FacebookDataFetcher.FetchData("search_query", "limit");
await FacebookDataFetcher.FetchDataFromPageUrl("https://www.facebook.com/pageurl", "limit");
await FacebookDataFetcher.FeedbackForPostsOfPageWithUrl("https://www.facebook.com/pageurl", "limit", ReactionsEnum.LOVE, "Your comment here");
```

## Configuration

You can configure various aspects of the solution through the `appsettings.json` file, including:

- **Facebook login credentials**: Your Facebook email and password for login.
- **Proxy settings**: Optionally, configure a proxy for your requests.
- **API endpoint**: The endpoint for Facebook's GraphQL API.

## Services & Dependencies

This project uses the following services and libraries:

- **Dependency Injection**: Managed through `IHostBuilder` to inject services into the application.
- **HttpClient Singleton**: A singleton service that handles HTTP client configuration for making API calls.
- **Facebook API**: The project interacts with Facebook's GraphQL API for post interactions and data fetching.

Key services include:
- `FacebookLoginAutomation`: Handles Facebook login.
- `ICommentsService`: Allows commenting on posts.
- `IReactionsService`: Handles reactions to posts.
- `IFacebookUserActionsService`: Handles actions like sending friend requests, following, and poking users.

## Contributing

We welcome contributions! If you'd like to improve this project or fix bugs, feel free to fork the repository, create a branch, and submit a pull request.

### How to contribute:

1. Fork the repository.
2. Create a new branch: `git checkout -b feature-name`.
3. Make your changes.
4. Commit your changes: `git commit -am 'Add new feature'`.
5. Push to the branch: `git push origin feature-name`.
6. Create a pull request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

Thank you for using the Facebook Automation Solution! If you encounter any issues or have questions, please feel free to open an issue or reach out to us. Enjoy automating your Facebook experience!
