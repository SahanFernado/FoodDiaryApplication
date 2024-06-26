var app = angular.module('FoodDiaryApp', []);

app.controller('UsersController', function ($scope, $http) {
    $scope.user = {};

    $scope.register = function () {
        $http({
            method: 'POST',
            url: '/Users/Register',
            data: $scope.user,
            headers: { 'Content-Type': 'application/json' }
        }).then(function (response) {
            alert('Registration successful');
            window.location.href = '/Users/Login';
        }, function (error) {
            console.log(error);
            alert('Registration failed');
        });
    };

    $scope.login = function () {
        
        $http({
            method: 'POST',
            url: '/Users/Login',
            data: $scope.user,
            headers: { 'Content-Type': 'application/json' }
        }).then(function (response) {
            
            if (response.data.success) {
                window.location.href = '/FoodDiary/CreateEntry';
            } else {
                alert('Login failed');
            }
        }, function (error) {
            alert('Login failed');
        });
    };
});

app.controller('FoodDiaryController', function ($scope, $http) {
    $scope.createEntry = function () {
        $http.post('/FoodDiary/CreateEntry', $scope.entry).then(function (response) {
            if (response.data.success) {
                alert('Entry created successfully');
                window.location.href = '/FoodDiary/Index';
            } else {
                alert('Failed to create entry');
            }

        }, function (error) {
            alert('Failed to create entry');
        });
    };
});