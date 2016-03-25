(function () {
    'use strict';

    angular.module('RunningCounterApp')
        .controller('LoginController', ['$scope', '$location', 'AuthenticationService', function ($scope, $location, authService) {

            $scope.loginError = false;
            $scope.loginData = {
                userName: '',
                password: ''
            };

            activate();

            function activate() {
                authService.logOut();
            };

            $scope.login = function () {
                if (!$scope.loginForm.$valid) {
                    return;
                }
                authService.login($scope.loginData).then(function (response) {
                    $location.path('/home');
                },
                 function (err) {
                     $scope.loginError = true;
                 });
            };
        }]);
})();