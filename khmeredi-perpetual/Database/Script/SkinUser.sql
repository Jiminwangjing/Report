
						--////////////////insert SkinUser(First)//////////////////////
insert into SkinUser(s.SkinID,s.UserID, s.Unable) 
select  c.ID, COALESCE(s.Unable, 0), COALESCE(s.UserID, 0) from SkinUser s right join  ColorSetting c on c.ID = s.SkinID 


--///////////////update SkinUser(secend)//////////////////////
insert into SkinUser(s.SkinID,s.UserID, s.Unable) 
select  COALESCE(s.SkinID,0),  COALESCE(a.ID,0),COALESCE(s.Unable, 0) from SkinUser s cross join  tbUserAccount a