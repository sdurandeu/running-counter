# ASP.NET MVC + Angular.js Sample: Running Counter

### How to Run the Solution
1. Open **code\RunningCounter.sln** using Visual Studio 2013/2015.
2. Press **F5** to run it. Entity Framework will create a new empty database for the app (and an `.mdf` file under the `App_Data` folder).

### Technologies and Frameworks

* ASP.NET MVC 5  + Web API 2.2
* Angular.js 1.4.3 + Angular UI Bootstrap
* Boostrap 3
* OAuth2 Resource Owner Password Credentials Grant
  - see [this post](http://bitoftech.net/2014/06/09/angularjs-token-authentication-using-asp-net-web-api-2-owin-asp-net-identity/) together with [this post](http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/)

### Code Style
* Angular.js: The code tries to follow the guidelines from John's Papa Style Guidelines: https://github.com/johnpapa/angular-styleguide
* C#: StyleCop 4.8

### TODO

* Change Time pickers for a Select with list of times every half-hour
* When editing meals inline use the Boostrap DatePicker instead of the standard one from the browser
* Loading/delete animations
* Show average kilometers per day
* Add "Remember Me" button (localStorage/sessionStorage)
* Add RequireHttps attribute in controllers
* Add JSONP CSRF blocking prefix

## Notes
* AntiForgery token validation is not necessary in this scenario as we are using explicit tokens for authentication


