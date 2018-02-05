app.controller('SearchByLinkCtrl', function ($scope, $http, $timeout) {

    InitializeFunction();

    //$scope.controls = {
    //    link: {
    //        required: true,
    //        minlength: 11,
    //        maxlength: 11
    //    }
    //}

    $scope.SearchClick = function () {

        $('#loading').show();

        $http({
            method: 'GET',
            url: '/api/searchbylink/Search/?searchText=' + $scope.link + "&downloadSource=" + $scope.downloadsource
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
            url: '/api/searchbylink/download',
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
            url: '/api/searchbylink/askforprogress'
        }).then(function successCallback(response) {
            $scope.videos = response.data;
            $timeout($scope.AskForProgress, 1000);
            $('#loading').hide();

        }, function errorCallback(response) {
        });
    }

    $scope.redirectToUrl = function (url) {
        window.open('https://www.youtube.com/watch?v=' + url, '_blank');
    };

    function InitializeFunction() {
        $scope.downloadsource = "1"; //YouTube
    }
});