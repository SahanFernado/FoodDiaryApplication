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
            console.log(response);
            if (response.data.success) {
                window.location.href = '/Home/Index';
            } else {
                alert('Login failed');
            }
        }, function (error) {
            alert('Login failed');
        });
    };
});