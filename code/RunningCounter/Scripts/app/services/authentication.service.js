(function () {
    'use strict';

    // inspired from http://bitoftech.net/2014/06/09/angularjs-token-authentication-using-asp-net-web-api-2-owin-asp-net-identity/
    angular.module('RunningCounterApp')
        .factory('AuthenticationService', ['$http', '$q', 'localStorageService', function ($http, $q, localStorageService) {

            var authentication = { isAuth: false, userName: null };
            var userSettings = { kilometersGoal: null, isAdmin: false };
            var LOCAL_STORAGE_AUTH_DATA_KEY = 'authorizationData';

            return {
                authentication: authentication,
                userSettings: userSettings,
                saveRegistration: saveRegistration,
                login: login,
                logOut: logOut,
                fillAuthData: fillAuthData,
                getUserSettings: getUserSettings,
                updateUserSettings: updateUserSettings
            };

            function saveRegistration(registration) {
                logOut();

                return $http.post('api/account/register', registration).then(function (response) {
                    return response;
                });
            };

            function login(loginData) {
                var data = 'grant_type=password&username=' + loginData.userName + '&password=' + loginData.password;

                var deferred = $q.defer();

                $http.post('/auth/token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

                    localStorageService.set(LOCAL_STORAGE_AUTH_DATA_KEY, { token: response.access_token, userName: loginData.userName });

                    authentication.isAuth = true;
                    authentication.userName = loginData.userName;

                    deferred.resolve(response);

                }).error(function (err, status) {
                    logOut();
                    deferred.reject(err);
                });

                return deferred.promise;
            };

            function logOut() {
                localStorageService.remove(LOCAL_STORAGE_AUTH_DATA_KEY);
                authentication.isAuth = false;
                authentication.userName = '';
            };

            function fillAuthData() {
                var authData = localStorageService.get(LOCAL_STORAGE_AUTH_DATA_KEY);
                if (authData) {
                    authentication.isAuth = true;
                    authentication.userName = authData.userName;
                }

                return authentication;
            };

            function getUserSettings() {
                var deferred = $q.defer();

                $http.get('api/account/settings').success(function (response) {
                    userSettings.kilometersGoal = response.kilometersGoal;
                    userSettings.isAdmin = response.isAdmin;
                    deferred.resolve(response);
                }).error(function (err, status) {
                    deferred.reject(err);
                });

                return deferred.promise;
            };

            function updateUserSettings(settings) {
                return $http.post('api/account/settings', settings).success(function (response) {
                    userSettings.kilometersGoal = settings.kilometersGoal;
                });
            };

        }]);

})();