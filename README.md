# log4net-appender-azurefilestorage
Log4net appender to write log entries to Azure Cloud File Storage.

## Usage

First, make sure you're familiar with log4net and how to configure it. There is a [great tutorial by Tim Corey](http://www.codeproject.com/Articles/140911/log4net-Tutorial) that'll get you up to speed.

To use the Azure cloud file storage appender, simply follow these steps:

1. Add a reference to **log4net.Appender.AzureFileStorage.dll** [nuget package](https://www.nuget.org/packages/log4net.Appender.AzureFileStorage.dll).

2. Setup your config file to configure log4net and the appender.

   Here's an example:

   ```xml
   <?xml version="1.0" encoding="utf-8" ?>
   <configuration>
     <configSections>
       <section name="log4net" type="log4net.Config.Log4NetConfigurationSectionHandler, log4net"/>
     </configSections>
     <log4net>
       <appender name="AzureFileAppender" type="log4net.Appender.AzureFileStorage.AzureFileAppender, log4net.Appender.AzureFileStorage">
         <AzureStorageConnectionString value="DefaultEndpointsProtocol=https;AccountName=mystorageaccountname;AccountKey=secretkeyxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx==" />
         <ShareName value="mylogs" />
         <Path value="thisapp" />
         <File value="log_{yyyy-MM-dd}.txt" />
         <layout type="log4net.Layout.PatternLayout">
           <ConversionPattern value="%date %-5level %logger %message%newline"/>
         </layout>
       </appender>
       <root>
         <level value="ALL" />
         <appender-ref ref="AzureFileAppender"/>
       </root>
     </log4net>
   </configuration>
   ```

3. Add the line that hooks up your assembly with log4net to the top of one of your classes outside of a namespace:

   ```
   [assembly: log4net.Config.XmlConfigurator(Watch = true)]
   ```

4. Add a line that sets up the logger inside each class you want to perform logging in:

   ```
   private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
   ```

5. Pepper your code with log writing statements, such as:

   ```c#
   log.Fatal("A fatal log entry.");
   log.Error("An error log entry.");
   log.Warn("A warning log entry.");
   log.Info("An informational log entry.");
   log.Debug("A debug log entry.");
   ```

## Specifics

Configuration parameters for the Azure cloud file appender are as follows:

* **AzureStorageConnectionString** set this to the connection string from your Azure account for the storage you want to use.
* **ShareName** set this to the name of the share to create within the storage account.
* **Path** set this to the path within the share.
* **File** set this to the log filename. You can embed a date within this parameter by specifying curly braces. 

   For example, setting the **File** parameter to `log_{yyyy-MM-dd}.txt`
   
   Will generate files like this:
   
   * log_2016-05-19.txt
   * log_2016-05-20.txt
