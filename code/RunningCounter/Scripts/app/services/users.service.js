(function () {
    'use strict';

    angular.module('RunningCounterApp')
        .factory('User', ['$resource', User]);

    function User($resource) {

        return $resource('/api/users/:id', null, {
            update: { method: 'PATCH' }
        });
    }

})();