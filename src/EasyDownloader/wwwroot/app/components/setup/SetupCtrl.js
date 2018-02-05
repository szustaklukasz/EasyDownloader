app.controller('SetupCtrl', function ($scope, $http, $timeout) {
    $scope.name = '';

    $scope.ButtonClick = function () {

        $http({
            method: 'GET',
            url: '/api/home/find/?searchText=' + $scope.name
        }).then(function successCallback(response) {
            $scope.YouTubeVideos = response.data;

        }, function errorCallback(response) {
            $scope.YouTubeVideos = response.data;
        });
    };

    $scope.DownloadClick = function() {

        $http({
            method: 'POST',
            url: '/api/home/download',
            data: $scope.YouTubeVideos
        }).then(function successCallback(response) {
            $scope.AskForProgress();

        }, function errorCallback(response) {

        });
    };

    $scope.AskForProgress = function() {

        $http({
            method: 'GET',
            url: '/api/home/askforprogress'
        }).then(function successCallback(response) {
            $scope.YouTubeVideos = response.data;
            $timeout($scope.AskForProgress, 1000);

        }, function errorCallback(response) {
        });
    };

    $scope.redirectToUrl = function (url) {
        window.open('https://www.youtube.com/watch?v=' + url, '_blank');
    };
});