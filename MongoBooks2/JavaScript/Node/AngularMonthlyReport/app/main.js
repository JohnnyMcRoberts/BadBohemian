(function () {
    'use strict';

    angular
        .module('app')
        .controller('Main', main);

    function main() {
        var vm = this;
        vm.food = 'pizza';
        vm.names = [
            { name: 'Jani', country: 'Norway' },
            { name: 'Hege', country: 'Sweden' },
            { name: 'Kai', country: 'Denmark' }
        ];

        vm.items = [
            {
                name: 'Jani', country: 'Norway',
                subitems: {
                    "car1": "Ford 02",
                    "car2": "BMW 2nd",
                    "car3": "Fiat *2"
                } },
            {
                name: 'Hege', country: 'Sweden',
                subitems: {
                    "car1": "Ford - 3",
                    "car2": "BMW 3",
                    "car3": "Fiat 33"
                } },
            {
                name: 'Kai', country: 'Denmark',
                subitems: {
                    "car1": "Ford 4",
                    "car2": "BMW44",
                    "car3": "Fiat 44"
                } }
        ];
    }

})();