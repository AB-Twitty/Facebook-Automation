# Facebook Automation Solution

This repository provides a comprehensive solution for automating various tasks on Facebook. It enables users to interact with Facebook through automation, allowing for seamless login, user actions, data fetching, and interaction with posts, pages, and groups.

The solution makes use of C# along with Selenium for Facebook login, handling proxy settings, performing actions like sending friend requests, poking, and following, as well as interacting with posts by commenting and reacting.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Requirements](#requirements)
- [Setup & Installation](#setup--installation)
- [Usage](#usage)
  - [Facebook Login Automation](#facebook-login-automation)
  - [User Actions](#user-actions)
  - [Fetching Facebook Data](#fetching-facebook-data)
  - [Reactions & Comments](#reactions--comments)
  - [Group and Page Interaction](#group-and-page-interaction)
- [Configuration](#configuration)
- [Services & Dependencies](#services--dependencies)
- [Contributing](#contributing)
- [License](#license)

## Overview

This Facebook Automation solution is designed to automate several tasks such as logging into Facebook, sending friend requests, following users, posting comments, reacting to posts, joining groups, and fetching data based on queries. 

The solution utilizes Selenium for login automation and supports proxy authentication. It also allows interaction with posts, pages, and groups by reacting to or commenting on content, as well as fetching user data, post feedback, and group membership details.

Key features include:
- Login with cookie validation or fresh login if cookies are expired.
- Proxy support for login via a configured username and password for proxy authentication.
- Fetching data from Facebook posts, pages, and groups based on a keyword, with support for limiting the number of results.
- Performing bulk user actions like friend requests, poking, and following.
- Reacting to posts and commenting on them.
- Group and page interaction for feedback actions, including commenting and reacting on posts.

## Features

- **Login Automation**: Validate and reuse cookies, log in using Selenium, and handle proxy authentication for the browser session.
- **User Actions**: Send friend requests, poke users, or follow users programmatically.
- **Post Interaction**: React and comment on posts, reply to comments, or interact with Facebook feedback (like reactions, shares, and comments).
- **Data Fetching**: Search Facebook posts, pages, and groups by keywords and fetch the people involved with posts (those who reacted, commented, or reshared).
- **Group & Page Feedback**: Add reactions and comments to posts within specified groups or pages.
- **Join Group**: Send a join request to a Facebook group.
  
## Requirements

- **.NET 6.0** or later
- **C# 10.0** or later
- **Selenium WebDriver**: For browser automation and login.
- **Visual Studio** (or any C# IDE of choice)
- **Facebook credentials**: Required for logging into Facebook.
- **Proxy details**: Optional, used for configuring proxy authentication.
- **Facebook API access**: Required for interacting with Facebook's GraphQL API.

## Setup & Installation

1. **Clone the repository**:

   ```bash
   git clone https://github.com/yourusername/facebook-automation.git
   cd facebook-automation
   ```

2. **Install dependencies**:

   If using Visual Studio, open the solution file (`FacebookAutomation.sln`). The IDE should restore all required dependencies automatically.

   Alternatively, if using the command line:

   ```bash
   dotnet restore
   ```

3. **Configure Application**:

   - Open the `appsettings.json` file and configure your Facebook credentials and proxy settings (if applicable).
   - Provide the proxy username and password for authentication when the browser prompt opens.

   Example:

   ```json
   {
     "FacebookAuth": {
       "Email": "your_email@example.com",
       "Password": "your_password",
       "ProxySettings": {
         "IpAddress": "proxy_ip",
         "Port": "proxy_port",
         "Username": "proxy_username",
         "Password": "proxy_password"
       }
     },
     "ApiUrl": "https://www.facebook.com/api/graphql/"
   }
   ```

4. **Run the application**:

   Build and run the application. In Visual Studio, press **F5** to start. Alternatively, use the command line:

   ```bash
   dotnet run
   ```

## Usage

### Facebook Login Automation

The solution first checks if there are saved cookies for the current session. If cookies exist and are valid, the system will load them to continue the session. If the cookies are expired or not found, it will proceed to log in using the provided Facebook credentials and proxy settings.

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

### User Actions

The solution allows for bulk user actions, such as sending friend requests, poking users, and following users:

```csharp
var userActionsService = new FacebookUserActionsService();
await userActionsService.SendBulkFriendRequests(new List<string> { "user_id" });
await userActionsService.PokeUser("user_id");
await userActionsService.FollowUser("user_id");
```

### Fetching Facebook Data

You can fetch Facebook data such as user feedback, posts, pages, and groups by specifying a keyword and limit. The solution will prioritize fetching data from posts, pages, and groups, in that order, and retrieve the people who interacted with the content (comments, reactions, and shares).

```csharp
await FacebookDataFetcher.FetchData("search_query", "limit");
await FacebookDataFetcher.FetchDataFromPageUrl("https://www.facebook.com/pageurl", "limit");
await FacebookDataFetcher.FeedbackForPostsOfPageWithUrl("https://www.facebook.com/pageurl", "limit", ReactionsEnum.LOVE, "Your comment here");
```

### Reactions & Comments

You can add reactions and comments to posts, or even react and comment on specific comments made by others.

```csharp
var reactionsService = new ReactionsService("https://www.facebook.com/api/graphql/", FormDataState.Instance.GetAllFormData());
await reactionsService.ReactTo(post.FeedbackId, ReactionsEnum.LOVE);

var commentsService = new CommentsService("https://www.facebook.com/api/graphql/", FormDataState.Instance.GetAllFormData());
await commentsService.CommentOn(post.FeedbackId, "Nice post!", true); // Respond to a comment
```

### Group and Page Interaction

For groups and pages, you can provide URLs and limits to interact with posts, such as adding reactions or comments. The solution will perform these actions across the specified number of posts within the group or page.

```csharp
await FacebookGroupsIntegrationService.SendJoinRequest(new GroupInfoModel
{
    Id = "group_id",
    Url = "https://www.facebook.com/groups/groupname"
});

await FacebookDataFetcher.FeedbackForPostsOfPageWithUrl("https://www.facebook.com/pageurl", "limit", ReactionsEnum.LOVE, "Great post!");
```

### Join Group

You can send a join request to a group using the following method:

```csharp
await FacebookGroupsIntegrationService.SendJoinRequest(new GroupInfoModel
{
    Id = "group_id",
    Url = "https://www.facebook.com/groups/groupname"
});
```

## Configuration

You can configure various aspects of the solution through the `appsettings.json` file, such as:
- **Facebook credentials**: Your Facebook email and password.
- **Proxy settings**: Optional, to authenticate via proxy.
- **API endpoint**: The endpoint for Facebook’s GraphQL API.

## Services & Dependencies

This project uses the following services and libraries:

- **Selenium WebDriver**: For automating the Facebook login.
- **HttpClient Singleton**: For making API calls to Facebook.
- **Dependency Injection**: For managing services such as user actions, comments, and reactions.
- **Facebook API**: To interact with Facebook posts, pages, and groups via GraphQL.

## Contributing

We welcome contributions! If you'd like to improve this project, fix bugs, or add new features, feel free to fork the repository, create a branch, and submit a pull request.

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

Thank you for using the Facebook Automation Solution! If you encounter any issues or have questions, feel free to open an issue or reach out. Enjoy automating your Facebook experience!
