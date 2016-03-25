(function () {
    'use strict';

    angular.module('RunningCounterApp')
        .controller('NavbarController', ['$scope', '$location', '$uibModal', 'AuthenticationService', function ($scope, $location, $uibModal, authService) {

            $scope.authentication = null;
            $scope.userSettings = null;
            $scope.manageUsers = manageUsers;
            $scope.logOut = logOut;
            $scope.openSettings = openSettings;

            activate();

            function activate() {
                $scope.authentication = authService.fillAuthData();
                $scope.userSettings = authService.userSettings;
            }

            function logOut() {
                authService.logOut();
                $location.path('/login');
            }

            function openSettings() {

                var modalInstance = $uibModal.open({
                    templateUrl: 'user-settings-modal.html',                    
                    resolve: {
                        settings: authService.userSettings
                    },
                    controller: ['$scope', '$uibModalInstance', 'settings', UserSettingsModalController]
                });

                modalInstance.result.then(function (kilometersGoal) {
                    var settings = {
                        kilometersGoal: kilometersGoal
                    }
                    authService.updateUserSettings(settings);
                });
            }

            function manageUsers() {
                $location.path('/users');
            }

            function UserSettingsModalController($scope, $uibModalInstance, settings) {
                $scope.kilometersGoal = settings.kilometersGoal;

                $scope.ok = function () {
                    $uibModalInstance.close($scope.kilometersGoal);
                };

                $scope.cancel = function () {
                    $uibModalInstance.dismiss('cancel');
                };
            }            

        }]);

})();