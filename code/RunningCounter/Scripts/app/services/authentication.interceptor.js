(function () {
    'use strict';

    angular.module('RunningCounterApp')
        .factory('AuthenticationInterceptor', ['$q', '$location', 'localStorageService', function ($q, $location, localStorageService) {

            return {
                request: request,
                responseError: responseError
            };

            function request(config) {
                config.headers = config.headers || {};

                var authData = localStorageService.get('authorizationData');
                if (authData) {
                    config.headers.Authorization = 'Bearer ' + authData.token;
                }

                return config;
            };

            function responseError(rejection) {
                if (rejection.status === 401) {
                    $location.path('/login');
                }
                return $q.reject(rejection);
            };

        }]);

})();