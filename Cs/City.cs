///程序生成文件,请勿自行更改!!!
///文件生成时间-2024/10/26 9:32:28
using System.Collections.Generic;
public class City
{
    public string ID {get;set;}
    public string CityName {get;set;}
    public int CityPop {get;set;}
    public int CityType {get;set;}
    public City(string ID ,string CityName ,int CityPop ,int CityType )
    {
        this.ID = ID;
        this.CityName = CityName;
        this.CityPop = CityPop;
        this.CityType = CityType;
    }

    public City(){
    
    }
}