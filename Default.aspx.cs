using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sage.SData.Client.Core;
using Sage.SData.Client.Extensions;
using Sage.SData.Client.Atom;
using System.Diagnostics;

public partial class _Default : System.Web.UI.Page
{
    const String SDATA_NS = "http://schemas.sage.com/dynamic/2007";

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        btnRunSample.Click += new EventHandler(btnRunSample_Click);
    }

    void btnRunSample_Click(object sender, EventArgs e)
    {
        var svc = GetSDataService(txtUrl.Text, txtUsername.Text, txtPassword.Text);
        
        int numAccounts = int.Parse(txtAccounts.Text);
        int numContacts = int.Parse(txtContacts.Text);
        int numOpportunities = int.Parse(txtOpportunities.Text);

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        ResourceLocator[] createdAccounts = CreateAccounts(svc, numAccounts, numContacts, numOpportunities);
        stopWatch.Stop();
        TimeSpan ts = stopWatch.Elapsed;
        txtResults.Text = String.Format("Total elapsed time: {0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        TimeSpan tsPerAccount = new TimeSpan(ts.Ticks / numAccounts);
        txtResults.Text += String.Format("\r\nAvg time per account: {0:00}:{1:00}:{2:00}.{3:00}",
            tsPerAccount.Hours, tsPerAccount.Minutes, tsPerAccount.Seconds, tsPerAccount.Milliseconds / 10);
        TimeSpan tsPerRecord = new TimeSpan(tsPerAccount.Ticks / (numContacts + numOpportunities * numContacts));
        txtResults.Text += String.Format("\r\nAvg time per record: {0:00}:{1:00}:{2:00}.{3:00}",
            tsPerRecord.Hours, tsPerRecord.Minutes, tsPerRecord.Seconds, tsPerRecord.Milliseconds / 10);

        foreach (ResourceLocator acc in createdAccounts)
        {
            DeleteResource(svc, "Account", acc);
        }
        
    }

    private ResourceLocator[] CreateAccounts(SDataService svc, int numAccounts, int numContacts, int numOpportunities)
    {
        ResourceLocator[] createdAccounts = new ResourceLocator[numAccounts];
        while (numAccounts > 0)
        {
            createdAccounts[numAccounts - 1] = CreateAccount(svc, numContacts, numOpportunities);
            numAccounts--;
        }
        return createdAccounts;
    }

    private ResourceLocator CreateAccount(SDataService ws, int numContacts, int numOpportunities)
    {
        Dictionary<String, object> accValues = new Dictionary<string, object>();
        accValues["AccountName"] = "Test Account";
        ResourceLocator account = CreateResource(ws, "Account", accValues);
        ResourceLocator[] contactIds = new ResourceLocator[numContacts];
        while (numContacts > 0)
        {
            contactIds[numContacts - 1] = CreateContact(ws, account.Id);
            numContacts--;
        }
        while (numOpportunities > 0)
        {
            CreateOpportunity(ws, account.Id, contactIds);
            numOpportunities--;
        }

        return account;
    }

    private ResourceLocator CreateContact(SDataService ws, String accountId)
    {
        Dictionary<String, object> conValues = new Dictionary<string, object>();
        conValues["Account"] = new SDataPayload { Key = accountId };
        conValues["LastName"] = "Test";
        conValues["FirstName"] = "Joe";
        return CreateResource(ws, "Contact", conValues);
    }

    private ResourceLocator CreateOpportunity(SDataService ws, String accountId, ResourceLocator[] contactIds)
    {
        Dictionary<String, object> oppValues = new Dictionary<string, object>();
        oppValues["Account"] = new SDataPayload { Key = accountId };
        oppValues["Description"] = "Test Opportunity";
        oppValues["Owner"] = new SDataPayload { Key = "SYST00000001" };

        SDataPayload[] contacts = new SDataPayload[contactIds.Length];
        int i = 0;
        foreach (ResourceLocator conId in contactIds)
        {
            SDataPayload oppCon = new SDataPayload { ResourceName = "OpportunityContact" };
            oppCon.Values["Contact"] = new SDataPayload { Key = conId.Id };
            contacts[i++] = oppCon;
        }
        oppValues["OpportunityContacts"] = contacts;
        return CreateResource(ws, "Opportunity", oppValues, "opportunities"); 
    }

    private void DeleteResource(SDataService ws, String resourceName, ResourceLocator resourceId)
    {
        var sru = new SDataSingleResourceRequest(ws);
        sru.ResourceKind = resourceName.ToLower() + "s";
        sru.ResourceSelector = "'" + resourceId.Id + "'";
        sru.Delete();
    }


    internal ResourceLocator CreateResource(SDataService ws, String resourceName, IDictionary<String, object> values, string resourceKind = null)
    {
        var sru = new SDataSingleResourceRequest(ws);
        sru.ResourceKind = resourceKind == null ? resourceName.ToLower() + "s" : resourceKind;
        SDataPayload payload = new SDataPayload();
        payload.ResourceName = resourceName;
        payload.Namespace = SDATA_NS;
        sru.Entry = new AtomEntry();
        foreach (KeyValuePair<String, object> kvp in values)
        {
            payload.Values[kvp.Key] = kvp.Value;
        }
        sru.Entry.SetSDataPayload(payload);
        AtomEntry entry = sru.Create();
        payload = entry.GetSDataPayload();
        return new ResourceLocator { Id = payload.Key, ETag = entry.GetSDataHttpETag() };
    }

    

    private SDataService GetSDataService(String url, String username, String password)
    {
        SDataService service = new SDataService();

        // set user name to authenticate with
        service.UserName = username;
        // set password to authenticate with
        service.Password = password;

        Uri sdataUri = new Uri(url);
        service.Protocol = sdataUri.Scheme;
        service.ServerName = sdataUri.Host;
        service.ApplicationName = "slx";
        service.ContractName = "dynamic";
        service.VirtualDirectory = sdataUri.AbsolutePath;
        return service;
    }

}