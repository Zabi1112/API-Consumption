using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApiUrls
{
    public const bool testBuild = true;


    private const string baseUrlTest = "https://pollenpop-dev.honey.land/api/";
    private const string baseUrlLive = "https://pollenpop.honey.land/api/";
    private const string baseUrlLocal = "http://127.0.0.1:8080/";

    //public const string baseUrl =testBuild? baseUrlTest : baseUrlLive;
    private const string baseUrl = baseUrlLocal;


    private const string baseGameUrlName = ""; //'testenv/'

    public const string validateToken = baseUrl + baseGameUrlName + "auth/v1/validateToken"; //Body  //Auth = N
    public const string refreshToken = baseUrl + baseGameUrlName + "auth/v1/refreshToken"; //Body  //Auth = N

    public const string getAllUsers = baseUrl + baseGameUrlName + "user/v1/getAllUsers"; //Query  //Auth = N
    public const string loginOrSignUpUrl = baseUrl + baseGameUrlName + "user/v1/loginOrSignUp"; //Query  //Auth = N
    public const string createUser = baseUrl + baseGameUrlName + "user/v1/createUser"; //Body  //Auth = N

    public const string getAllStats = baseUrl + baseGameUrlName + "lb/v1/getMHAllStats"; //Body  //Auth = N


    // dummu urls fro leaderBoard
    public const string DummyUrl = "https://dummyjson.com/products/1";
    public const string DummyUrl2 = "https://dummyjson.com/products";
}