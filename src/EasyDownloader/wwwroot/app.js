var app = angular.module("app", ['ngRoute']);

app.config(['$locationProvider', function ($locationProvider) {
    $locationProvider.hashPrefix("");
}]);

app.config(function ($routeProvider) {
    $routeProvider.
    when('/home', {
        templateUrl: 'app/components/home/home.html',
        controller: 'HomeCtrl'
    }
    ).
    when('/searchbyphrase', {
        templateUrl: 'app/components/searchbyphrase/searchbyphrase.html',
        controller: 'SearchByPhraseCtrl'
    }).
    when('/searchbylink', {
        templateUrl: 'app/components/searchbylink/searchbylink.html',
        controller: 'SearchByLinkCtrl'
    }).
    when('/setup', {
        templateUrl: 'app/components/setup/setup.html',
        controller: 'SetupCtrl'
    }).
    otherwise({
        templateUrl: 'app/components/home/home.html',
        controller: 'HomeCtrl'
    });

    $('#loading').hide();
});