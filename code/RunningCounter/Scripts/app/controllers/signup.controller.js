(function () {
    'use strict';

    angular.module('RunningCounterApp')
        .controller('SignupController', ['$scope', '$location', 'AuthenticationService', function ($scope, $location, authService) {

            $scope.savedSuccessfully = false;
            $scope.errorMessages = null;
            $scope.registration = {
                userName: '',
                password: '',
                confirmPassword: ''
            };

            $scope.signUp = function () {

                if (!$scope.signupForm.$valid) {
                    return;
                }

                authService.saveRegistration($scope.registration).then(function (response) {
                    $scope.savedSuccessfully = true;
                    $scope.errorMessages = null;
                },
                 function (response) {
                     var errors = [];
                     for (var key in response.data.modelState) {
                         for (var i = 0; i < response.data.modelState[key].length; i++) {
                             errors.push(response.data.modelState[key][i]);
                         }
                     }
                     $scope.errorMessages = errors.join(' ');
                 });
            };

            $scope.browseLogin = function () {
                $location.path('/login');
            };
          
        }]);

})();