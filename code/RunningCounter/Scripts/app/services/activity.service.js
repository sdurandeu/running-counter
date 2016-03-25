(function () {
    'use strict';

    angular.module('RunningCounterApp')
        .factory('Activity', ['$resource', function ($resource) {

            return $resource('/api/activities/:id', null, {
                update: { method: 'PUT' }
            });
        }]);
})();