
class Warehouse extends Map
{
    constructor(option = 0){
        super();
        self = this;
        if(this.isValidObject(option)){
            this.addTable(option.table, option.jsons, option.key);
        } 
        
        Map.prototype.find = super.get;
        Warehouse.prototype.table = this.map;
        Warehouse.prototype.from = this.array;
        Warehouse.prototype.select = this.array;

        //Filter for objects by specified condition.
        Array.prototype.where = function(condition){
            return $.grep(this, function(json, i){
                return condition(json, i);
            });
        };

        //Find single object from Array by specified condition.
        Array.prototype.first = function(condition){
            return $.grep(this, function(json, i){
                return condition(json, i);
            })[0];
        };

        //Iterate through each of values in array.
        Array.prototype.each = function(callback){
            return $.each(this, function(i, v){
                return callback(i, v);
            });
        }
      
        //Let any object could be inserted into map as extension method.
        Map.prototype.insert = function(value, key){
            if(key !== undefined){
                if(self.isValidObject(value)){
                    this.set(value[key], value); 
                }

                if(self.isValidArray(value)){
                    for(let item of value){
                        this.insert(item, key);
                    }
                }
            } else {
                if(self.isValidArray(value)){
                    this.insert(value[0], value[1]);
                }
            }   
        };

        //Extension method for modifying object.
        Map.prototype.update = function(value, key){
            if(key !== undefined){
                if (self.isValidObject(value)){
                    if(this.has(value[key])){
                        this.set(value[key], value);  
                    } else {
                        throw new Error("Object[" + key + " : " + value[key] + "] has not matched.");
                    }            
                }
                
                if (self.isValidArray(value)){      
                    for(let item of value){
                        this.update(item, key);               
                    }
                } 
            } else {
                if (self.isValidArray(value)){
                    this.update(value[0], value[1]);
                }
            }     
        }      
    }

    //Using Map to add new table to warehouse
    addTable(table, jsons, key){
        if(Array.isArray(jsons) && key !== undefined){
            if(this.get(table) === undefined){
                this.set(table, new Map());
            }

            for(let json of jsons){
                if(this.isValidObject(json)){
                    this.get(table).set(json[key], json);
                }        
            }
        }
    }

    //Use async promise as iteration param.
    async arrayAsync(table, promise){
        if(this.array(table).length > 0){
            return await Promise.all(this.array(table)).then(function(values){
                return promise(values);      
            });
        } else {
            console.error("Cannot iterate through empty array.");
        }
        
    }

    //Select all object in array value, if async param is defined then use the method 
    //as asynchronous iteration otherwise, as normal iteration.
    array(table, promise){
        if(promise!== undefined){
            return this.arrayAsync(table, promise);
        } else {
            let data = [];
            if(table !== undefined && this.has(table)){ 
                data = [...this.get(table).values()];
                if(data.length === 0){ return 0; }
                return data;
            }
            return 0;
        } 
    }

    map(table){
        if(this.get(table) === undefined){
            this.set(table, new Map());  
            setTimeout(() => {
                if(this.get(table).size === 0){
                    this.delete(table);
                } 
            }, 60000);     
        } 
       
        return this.get(table);
    }

    //Copy objects(s) from one table to another by specified key(s).
    async copy(from_table, to_table, data){
        if(this.get(from_table) === undefined){
            console.error("Source is invalid.");
            return;
        }

        if(this.get(to_table) === undefined){
            this.set(to_table, new Map());
        }

        if(data === undefined){
            this.set(to_table, this.get(from_table));            
        } 

        let item = {};
        if(!this.isValidArray(data)){
            if(!this.isValidObject(data)){
                let key = data;
                if((item = this.get(from_table).get(key)) !== undefined){    
                    await this.get(table).set(key, item);                  
                } 
            }        
        } else {
            let value = data[0];
            let key_name = data[1];
            if(!this.isValidObject(value)){
                for(let k of data){
                    this.copy(from_table, to_table, k);
                }
            } 
            
            if(data.length === 2){    
                if(this.isValidObject(value)){
                    await this.copy(from_table, to_table, value[key_name]);
                }

                if(this.isValidArray(value)){
                    if(this.isValidObject(value[0])){
                        for(let json of value){
                            this.copy(from_table, to_table, json[key_name]);
                        }
                    }
                    
                }

            }
    
        }

    }

    //Move objects(s) from one table to another by specified key(s).
    async cut(from_table, to_table, key){
        if(key === undefined){
            this.copy(from_table, to_table);
            let copied = false;
            for(let e of this.get(from_table).keys()){
                copied = this.get(to_table).has(e);      
            }

            if(copied){
               this.set(from_table, new Map());
            }
        } 

        if(!this.isValidArray(key)){
            this.copy(from_table, to_table, key);
            await this.get(from_table).delete(key);
        }

        if(this.isValidArray(key)){
            for(let k of key){
                this.cut(from_table, to_table, k);
            }
        } 
   
    }

    //Delete object from table by specified key(s).
    async remove(table, key){
        if(!this.isValidArray(key)){
            if(this.get(table).has(key)){
                await this.get(table).delete(key);
            }
        }

        if(this.isValidArray(key)){
            for(let k of key){
                this.remove(table, k);
            }
        }
    }

    //Insert an object into table.
    async insert(table, value, key){
        if(key !== undefined){
            if(this.get(table) === undefined){
                this.set(table, new Map());                
            } 

            if(this.isValidObject(value)){
                await this.get(table).set(value[key], value); 
            }

            if(this.isValidArray(value)){
                for(let item of value){
                    this.insert(table, item, key);
                }
            }
        } else {
            if(this.isValidArray(value)){
                await this.insert(table, value[0], value[1]);
            }
        }
    }

    //Modify existing object in table.
    async update(table, value, key){    
        if(key !== undefined){
            if(this.isValidObject(value)){
                if(this.get(table).has(value[key])){
                    await this.get(table).set(value[key], value);  
                } else {
                    throw new Error("Object["+ key +" : "+ value[key] +"] has not matched.");
                }            
            }
            
            if(this.isValidArray(value)){      
                for(let item of value){
                    this.update(table, item, key);               
                }
            } 
        } else {
            if(this.isValidArray(value)){
                await this.update(table, value[0], value[1]);
            }
        }     
    }
    
    //Search valid json objects by specified condition.
    async filter(table, condition){
        if(table !== undefined){
            if(typeof condition === "function"){
                return await $.grep(this.from(table), function(json, i){
                    return condition(json, i);
                });
            }
        }
    }

    searchText(table, column, value, equal = 0){
        return $.grep(this.from(table), function(json, i){
            if(typeof json[column] === "string" && typeof value === "string"){
                if(typeof equal !== undefined && option.equal){
                    return json[column].toLowerCase() === value.toLowerCase();
                } else {
                    return (json[column].toLowerCase()).includes(value.toLowerCase());
                }        
            }
        });
    }
    
    distinct(list, key){
       return list.filter((json, index) => {
           return list.map(json => json[key]).indexOf(json[key]) === index;
       });
    }

    //Condition checking methods
    isObject(value) {
        return !!value && value.constructor === Object;
    }

    isValidObject(value) {
        return !!value && value.constructor === Object && Object.getOwnPropertyNames(value).length > 0;
    }

    isArray(value) {
        return !!value && Array.isArray(value);
    }

    isValidArray(value) {
        return !!value && Array.isArray(value) && value.length > 0;
    } 
}







