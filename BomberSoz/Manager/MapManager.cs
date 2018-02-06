using BomberSoz.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BomberSoz.Manager
{
  public  class MapManager
    {
      
        public MapManager (string baseDirectory,string startMapName)
        {
            _baseDirectoryToMap = baseDirectory;
            _startMapName = startMapName;
             GetMapsFromDirectory();
         
        }
        private Random _rand = new Random((int)DateTime.Now.Ticks);
        private List<String> _mapsCollection;
        private string _baseDirectoryToMap;
        private string _startMapName;
        private string _currentPath { get; set; }


        /// <summary>
        /// получаем список карт из директории
        /// </summary>
        public void GetMapsFromDirectory()
        {
            _mapsCollection = new List<string>();
            _mapsCollection.AddRange(Directory.GetFiles(_baseDirectoryToMap).ToList<string>());
        
        }


       /// <summary>
       ///  выбираем из списка карту , возвращаем её,затем удаляем
       /// </summary>
       /// <returns></returns>
        public string GetNextMap ()
        {
            int randomIndex = _rand.Next(_mapsCollection.Count);
            _currentPath =_mapsCollection[randomIndex];
            _mapsCollection.Remove(_currentPath);


            return _currentPath;
        }



        /// <summary>
        /// возвращает карту,указанную в конфиге в параметре pathToMapFile
        /// </summary>
        /// <returns></returns>
        public string GetFirstMap()
        {
            _mapsCollection.Remove(_startMapName);
            return _startMapName;

        }
        
        /// <summary>
        /// если неиспользованные карты ещё есть возвращает true, иначе false
        /// </summary>
        /// <returns></returns>
        public bool CheckAvailableMap()
        {

            return _mapsCollection.Count > 0; 
        }

    }
}
