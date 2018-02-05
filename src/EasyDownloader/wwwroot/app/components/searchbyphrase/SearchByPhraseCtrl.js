app.controller('SearchByPhraseCtrl', function ($scope, $http, $timeout) {
    
    InitializeFunction();
    $scope.SearchClick = function () {

        $('#loading').show();

        $http({
            method: 'GET',
            url: '/api/searchbyphrase/Search/?searchText=' + $scope.name + "&downloadSource=" + $scope.downloadsource
        }).then(function successCallback(response) {
            $scope.videos = response.data;
            $('#loading').hide();

        }, function errorCallback(response) {
            $scope.videos = response.data;
        });
    };

    $scope.DownloadClick = function () {

        $http({
            method: 'POST',
            url: '/api/searchbyphrase/download',
            data: $scope.videos
        }).then(function successCallback(response) {
            $scope.AskForProgress();
            $('#loading').hide();

        }, function errorCallback(response) {

        });
    }

    $scope.AskForProgress = function () {

        $http({
            method: 'GET',
            url: '/api/searchbyphrase/askforprogress'
        }).then(function successCallback(response) {
            $scope.videos = response.data;
            $timeout($scope.AskForProgress, 1000);
            $('#loading').hide();

        }, function errorCallback(response) {
        });
    }

    $scope.updateSelectedOfVideos = function () {

        $http({
            method: 'POST',
            url: '/api/searchbyphrase/updateselectedofvideos',
            data: $scope.videos
        }).then(function successCallback(response) {
            $timeout($scope.updateSelectedOfVideos, 1000);
            $('#loading').hide();

        }, function errorCallback(response) {
        });
    }

    $scope.redirectToUrl = function (url) {
        window.open('https://www.youtube.com/watch?v=' + url, '_blank');
    };

    function InitializeFunction() {
        $scope.name = '';
        $scope.downloadsource = "1"; //YouTube
    }
});