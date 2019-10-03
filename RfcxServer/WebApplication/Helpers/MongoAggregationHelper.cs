using System;
using MongoDB.Bson;
using System.Linq;

namespace WebApplication.Helpers
{
    public class MongoAggregationHelper
    {

        /**
            Given a timestamp return a Datetime of the date that the 
            timestamps belongs. With hour, minutes and seconds set to 0.
         */
        public static DateTime getUtcDateFromTimestampInSeconds(long timestamp){
            DateTime dateAtStartOfUnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0,DateTimeKind.Utc);
            DateTime date_local             = dateAtStartOfUnixEpoch.AddMilliseconds(timestamp*1000);
            DateTime date_utc               = DateTime.SpecifyKind(date_local, DateTimeKind.Utc);
            DateTime date_utc_at_zero_time  = GetDateAtZeroTime(date_utc);       
            return date_utc_at_zero_time;
        }

        /**
            Given a Datetime , returns a new Datetime with same day, hour and month.
            But hour, minutes and seconds set to 0
         */
        public static DateTime GetDateAtZeroTime(DateTime date){
            /**
                important create Datetime inside .SpecifyKind constructor, to get a utc date at 0,0,0 time
            */
            return DateTime.SpecifyKind(new DateTime(date.Year, date.Month, date.Day, 0, 0, 0) , DateTimeKind.Utc);
        }

        public static DateTime[] buildDateSpan(DateTime startDate, DateTime endDate){
            DateTime[] datesSpan = Enumerable.Range( 0 , endDate.Subtract(startDate).Days )
                                    .Select(offset => startDate.AddDays(offset) )
                                    .ToArray();
            return datesSpan;

        }
        
        public static BsonDocument buildFillMissingDatesStage(DateTime[] datesSpan){

            BsonDocument fillMissingDatesStage = new BsonDocument {
                { "$project" , new BsonDocument{
                        { "points", new BsonDocument {
                            { "$map" , new BsonDocument {
                                { "input" , new BsonArray(datesSpan) },
                                { "as" , "date"},
                                { "in", new BsonDocument{
                                    { "$let" , new BsonDocument{
                                        { "vars", new BsonDocument{
                                            { "dateIndex", new BsonDocument{
                                                { "$indexOfArray" , new BsonArray{ "$points.date", "$$date" } }
                                              }
                                            }
                                          }
                                        },
                                        { "in", new BsonDocument{
                                            { "$cond" , new BsonDocument{
                                                { "if" , new BsonDocument{
                                                    { "$ne" , new BsonArray { "$$dateIndex", -1 } }
                                                  }
                                                },
                                                { "then" , new BsonDocument{
                                                    { "$arrayElemAt" , new BsonArray { "$points", "$$dateIndex" } }                                  
                                                  }
                                                },
                                                { "else" , new BsonDocument { 
                                                    { "date" , "$$date"},
                                                    { "average", -1 }
                                                  }
                                                }
                                              }
                                            }
                                          }
                                        }
                                      }
                                    }
                                  }
                                }
                              }
                            }
                          }
                        }
                    }
                }
            };             

            return fillMissingDatesStage;
        }

        public static BsonDocument buildAddDateFieldStage(){
            BsonDocument unwindStage = BsonDocument.Parse(
                @"
                    {
                        $addFields : {
                            date : { 
                                $toDate : {
                                    $multiply : [ '$Timestamp', 1000 ]
                                }
                            }
                        }  
                    }
                "
            );
            return unwindStage;
        }

        public static BsonDocument buildGroupByDateStage(){
            BsonDocument groupByDateStage = BsonDocument.Parse(
                @"
                    {
                        $group : {
                            _id : {
                                year      : { $year       : '$date' },
                                month     : { $month      : '$date' },
                                dayOfMonth: { $dayOfMonth : '$date' }
                            },
                            average : { 
                                $avg :'$Value'
                            }
                        }
                    }
                "                             
            );
            return groupByDateStage;
        }

        public static BsonDocument buildProjectToDateStage(){
            BsonDocument projectToDateStage = BsonDocument.Parse(
                @"
                    {
                        $project : {
                            _id  : 0 ,
                            date : {
                                $dateFromParts : {
                                    year  : '$_id.year',
                                    month : '$_id.month',
                                    day   : '$_id.dayOfMonth',
                                }
                            }                                                       
                            average: 1
                        }
                    }
                "                             
            );
            return projectToDateStage;
        }

        
        //This stage is required for the stage to fill missing values
        public static BsonDocument buildShrinkToOneArrayStage(){
             BsonDocument shrinkToOneArrayStage = BsonDocument.Parse(
                @"
                    {
                        $group: {
                            _id: null,
                            points: { $push: '$$ROOT' }
                        }
                    }
                "
            );
            return shrinkToOneArrayStage;
        }

        // after the stage of missing values, expand values
        public static BsonDocument buildUnwindStage(){
            BsonDocument unwindStage = BsonDocument.Parse(
                @"
                    {
                        $unwind: '$points'
                    }
                "
            );
            return unwindStage;
        }  

        //promote original object to top level
        public static BsonDocument buildPromoteOneLevelStage(){
            BsonDocument promoteOneLevelStage = BsonDocument.Parse(
                @"
                    {
                        $replaceRoot: {
                            newRoot: '$points'
                        }
                    }
                "
            );
            return promoteOneLevelStage;
        }      
    }    
}