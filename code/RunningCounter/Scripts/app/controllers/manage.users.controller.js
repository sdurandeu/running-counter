(function () {
    'use strict';

    angular.module('RunningCounterApp')
        .controller('ManageUsersController', ['$scope', '$uibModal', 'User', ManageUsersController]);

    function ManageUsersController($scope, $uibModal, User) {

        var NEW_PASSWORD = '123456';

        $scope.users = User.query();
        $scope.deleteUser = deleteUser;
        $scope.resetPassword = resetPassword;
        $scope.passwordResetSuccess = false;
        $scope.deleteUserSuccess = false;
        $scope.newPassword = NEW_PASSWORD;
        $scope.showDeleteUserConfirmation = showDeleteUserConfirmation;

        function deleteUser(user) {
            User.delete({ id: user.id }, function () {
                $scope.deleteUserSuccess = true;

                $scope.users.splice($scope.users.indexOf(user), 1);
            });
        }

        function resetPassword(user) {
            var userPasswordUpdate = {
                password: NEW_PASSWORD
            };

            User.update({ id: user.id }, userPasswordUpdate, function () {
                $scope.passwordResetSuccess = true;
            });
        }

        function showDeleteUserConfirmation(user) {

            var modalInstance = $uibModal.open({
                templateUrl: 'delete-user-confirmation-modal.html',
                controller: ['$scope', function ($scope) {
                    $scope.cancel = function () {
                        modalInstance.dismiss();
                    };
                    $scope.ok = function () {
                        modalInstance.close();
                    };
                }]
            });

            modalInstance.result.then(function () {
                $scope.deleteUser(user);
            });
        }
    }
})();