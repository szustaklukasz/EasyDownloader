app.controller('IndexCtrl', function ($scope, $location, $http, $timeout) {

    $scope.goto = function (page) {
        $('#loading').show();
        $location.path(page);
        $('#loading').hide();
    };
});