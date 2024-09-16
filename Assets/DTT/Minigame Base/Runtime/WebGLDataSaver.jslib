mergeInto(LibraryManager.library, {
  SaveDataToLocalStorage: function (dataToSave, key){
	  localStorage.setItem('levelSelectData_' + UTF8ToString(key), UTF8ToString(dataToSave));
  },

  LoadDataFromBrowser: function (key){
	  const returnData = localStorage.getItem('levelSelectData_' + UTF8ToString(key));

    if(!returnData)
      return returnData
    
    // Get size of the string.
    var bufferSize = lengthBytesUTF8(returnData) + 1;
    
    // Allocate memory space.
    var buffer = _malloc(bufferSize);

    // Copy old data to the new one then return it.
    stringToUTF8(returnData, buffer, bufferSize);

    return buffer;
  },
});