(function () {
    'use strict';

    angular.module('RunningCounterApp', ['ngRoute', 'ngResource', 'ngAnimate', 'LocalStorageModule', 'ui.bootstrap'])
        .config(['$routeProvider', function ($routeProvider) {

            // define routes
            $routeProvider.when('/activities', {
                controller: 'ActivitiesController',
                templateUrl: 'scripts/app/views/activities.html'
            })
            .when('/login', {
                controller: 'LoginController',
                templateUrl: 'scripts/app/views/login.html'
            })
            .when('/signup', {
                controller: 'SignupController',
                templateUrl: 'scripts/app/views/signup.html'
            })
            .when('/users', {
                controller: 'ManageUsersController',
                templateUrl: 'scripts/app/views/manage-users.html'
            })
            .otherwise({ redirectTo: '/activities' });

        }]);

    angular.module('RunningCounterApp').config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push('AuthenticationInterceptor');
    }]);

})();