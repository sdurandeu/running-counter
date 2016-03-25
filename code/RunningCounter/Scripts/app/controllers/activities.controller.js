(function () {
    'use strict';

    angular.module('RunningCounterApp')
        .controller('ActivitiesController', ['$scope', '$uibModal', '$filter', 'Activity', 'AuthenticationService', function ActivitiesController($scope, $uibModal, $filter, Activity, authService) {

            $scope.newActivity = new Activity();
            $scope.newActivity.date = new Date();
            $scope.originalActivity = null;
            $scope.editedActivity = null;
            $scope.userSettings = authService.userSettings;
            $scope.todayKilometers = null;
            $scope.averageKilometers = 0;
            $scope.activities = [];
            $scope.filteredActivities = []; // created by using filter in html
            // date pickers
            $scope.datePickerAddOpened = false;
            $scope.datePickerFromOpened = false;
            $scope.datePickerToOpened = false;
            $scope.filterDateFrom = null;
            $scope.filterDateTo = null;
            $scope.filterTimeFrom = null;
            $scope.filterTimeTo = null;

            activate();

            function activate() {
                // get all activities
                $scope.activities = $scope.filteredActivities = Activity.query();

                authService.getUserSettings();
            }

            $scope.$watch('activities', function () {
                // calculate today's activities when the full collection changes
                $scope.todayKilometers = 0;
                angular.forEach($scope.activities, function (activity) {
                    var todayStart = new Date();
                    todayStart.setHours(0, 0, 0, 0);
                    if (new Date(activity.date) >= todayStart) {
                        $scope.todayKilometers += activity.kilometers;
                    };
                });
            }, true);

            $scope.$watch('filteredActivities', function () {
                // calculate average kilometers when the filtered collection changes
                var kilometersSum = 0;
                angular.forEach($scope.filteredActivities, function (activity) {
                    kilometersSum += activity.kilometers;
                });

                $scope.averageKilometers = kilometersSum > 0 ? Math.floor(kilometersSum / $scope.filteredActivities.length) : 0;
            }, true);

            $scope.deleteActivity = function (activity) {
                Activity.delete({ id: activity.id });

                $scope.activities.splice($scope.activities.indexOf(activity), 1);
            };

            $scope.addActivity = function () {
                if (!$scope.newActivityForm.$valid) {
                    return;
                }

                $scope.newActivity.$save(function (newActivity) {
                    $scope.activities.push(newActivity);
                    $scope.newActivity = new Activity();
                    $scope.newActivity.date = new Date();
                });
            };

            $scope.editActivity = function (activity) {
                if ($scope.editedActivity != null) {
                    $scope.cancelEdit($scope.editedActivity);
                }

                $scope.originalActivity = angular.extend({}, activity); // Clone the original todo to restore it on demand
                $scope.editedActivity = activity;
                $scope.editedActivity.parsedDate = new Date(activity.date);
            };

            $scope.saveEditedActivity = function (activity) {
                if (!activity.title || !activity.kilometers || !activity.parsedDate) {
                    return;
                }

                activity.date = activity.parsedDate.toJSON();
                delete activity.parsedDate;

                Activity.update({ id: activity.id }, activity, function () {
                    $scope.editedActivity = null;
                    $scope.originalActivity = null;
                });
            };

            $scope.cancelEdit = function (activity) {
                $scope.activities[$scope.activities.indexOf(activity)] = $scope.originalActivity;
                $scope.editedActivity = null;
                $scope.originalActivity = null;
            };

            $scope.getRemainingKilometers = function () {
                return ($scope.userSettings.kilometersGoal - $scope.todayKilometers) > 0 ? $scope.userSettings.kilometersGoal - $scope.todayKilometers : 0;
            };

            $scope.resetFilters = function () {
                $scope.filterDateFrom = null;
                $scope.filterDateTo = null;
                $scope.filterTimeFrom = null;
                $scope.filterTimeTo = null;
            };

            $scope.showDeleteConfirmation = function (activity) {

                var modalInstance = $uibModal.open({
                    templateUrl: 'activity-delete-confirmation-modal.html',
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
                    $scope.deleteActivity(activity);
                });
            };
        }]);


    angular.module('RunningCounterApp')
        .filter('dateFromTofilter', function () {
            return function (items, dateFrom, dateTo, timeFrom, timeTo) {
                var parsedDateFrom = new Date(dateFrom);
                var parsedDateTo = new Date(dateTo);
                var parsedTimeFrom = parseInt(timeFrom);
                var parsedTimeTo = parseInt(timeTo);

                if (isNaN(parsedTimeFrom) || parsedTimeFrom > 23) {
                    parsedTimeFrom = null;
                };

                if (isNaN(parsedTimeTo) || parsedTimeTo > 23) {
                    parsedTimeTo = null;
                };

                var result = [];
                for (var i = 0; i < items.length; i++) {
                    var itemDate = new Date(items[i].date);
                    if ((!dateFrom || itemDate > parsedDateFrom)
                         && (!dateTo || itemDate < parsedDateTo)
                         && (!parsedTimeFrom || itemDate.getHours() >= parsedTimeFrom)
                         && (!parsedTimeTo || itemDate.getHours() <= parsedTimeTo)) {
                        result.push(items[i]);
                    };
                };

                return result;
            };
        });
})();