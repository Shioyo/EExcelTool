///程序生成文件,请勿自行更改!!!
///文件生成时间-2024/10/14 7:56:02
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

    /*public List<City> GetList(){
        var listRow = ExcelReader.ReadList("City");
        List<City> returnList=new();
        for(int i=0;i<listRow.Count();i++){
            City item = new City();
            item.ID = listRow[i][0].GetValue<string>(); 
            item.CityName = listRow[i][1].GetValue<string>(); 
            item.CityPop = listRow[i][2].GetValue<int>(); 
            item.CityType = listRow[i][3].GetValue<int>(); 
        }
        return null;
    }*/
}