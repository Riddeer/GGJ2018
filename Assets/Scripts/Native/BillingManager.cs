using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;

public class BillingManager : MonoBehaviour
{
    public static BillingManager instance { get; private set; }

    public Dictionary<string, BillingProduct> m_Products =
        new Dictionary<string, BillingProduct>();

    private LoadingLayer m_LoadingLayer;
    private DialogLayer m_DialogLayer;

    private void OnEnable()
    {
        if (BillingManager.instance == null)
        {
            BillingManager.instance = this;
        }
        else if (BillingManager.instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        // Register for callbacks
        Billing.DidFinishRequestForBillingProductsEvent += OnDidFinishProductsRequest;
        Billing.DidFinishProductPurchaseEvent += OnDidFinishTransaction;

        // For receiving restored transactions.
        Billing.DidFinishRestoringPurchasesEvent += OnDidFinishRestoringPurchases;


        // create product
        m_Products.Clear();
        m_Products.Add(
            Constants.ProductID_iOS_Coin_50,
            this.CreateProduct(Constants.ProductID_iOS_Coin_50, true, 
                Constants.ProductID_iOS_Coin_50));
        m_Products.Add(
            Constants.ProductID_iOS_Hero_01,
            this.CreateProduct(Constants.ProductID_iOS_Hero_01, false, 
                Constants.ProductID_iOS_Hero_01));

        // send request to AppStore
        this.RequestBillingProducts();
    }

    private void OnDisable()
    {
        // Deregister for callbacks
        Billing.DidFinishRequestForBillingProductsEvent -= OnDidFinishProductsRequest;
        Billing.DidFinishProductPurchaseEvent -= OnDidFinishTransaction;
        Billing.DidFinishRestoringPurchasesEvent -= OnDidFinishRestoringPurchases;
    }

    void Start()
    {
        this.GetLoadingAndDialogLayer();
    }

    private BillingProduct CreateProduct(string name, bool isConsumable, string id)
    {
        PlatformValue[] platformVals = new PlatformValue[]
            {
                PlatformValue.IOS(id)
            };

        return BillingProduct.Create(name, isConsumable, platformVals);
    }

    private void GetLoadingAndDialogLayer()
    {
        if (m_LoadingLayer == null)
        {
            GameObject go =
               GameObject.FindGameObjectWithTag(Constants.SceneObject_LoadingLayer);
            if (go != null)
            {
                m_LoadingLayer = go.GetComponent<LoadingLayer>();
            }
        }

        if (m_DialogLayer == null)
        {
            GameObject go =
               GameObject.FindGameObjectWithTag(Constants.SceneObject_DialogLayer);
            if (go != null)
            {
                m_DialogLayer = go.GetComponent<DialogLayer>();
            }
        }
    }

    public void RequestBillingProducts()
    {
        Debug.Log("Request billing products.");

        NPBinding.Billing.RequestForBillingProducts(NPSettings.Billing.Products);

        // At this point you can display an activity indicator to inform user that task is in progress
        // m_LoadingLayer.SetActive(true);

    }

    private void OnDidFinishProductsRequest(BillingProduct[] _regProductsList, string _error)
    {
        Debug.Log("Receive billing products.\n" +
        "error :" + _error);
        // Hide activity indicator

        // Handle response
        if (_error != null)
        {
            // Something went wrong
        }
        else
        {
            // Inject code to display received products
            m_Products.Clear();
            foreach (BillingProduct one in _regProductsList)
            {
                if (!m_Products.ContainsKey(one.ProductIdentifier))
                    m_Products.Add(one.ProductIdentifier, one);
                Debug.Log("Receive billing products.\n" + 
                    "ProductID : " + one.ProductIdentifier);
            }
        }
    }

    public void RestoreCompletedTransactions()
    {
        // if (m_LoadingLayer != null) m_LoadingLayer.SetVisible(true);

        NPBinding.Billing.RestoreCompletedTransactions();
    }

    private void OnDidFinishRestoringPurchases(BillingTransaction[] _transactions, string _error)
    {
        if (m_LoadingLayer != null) m_LoadingLayer.SetVisible(false);

        Debug.Log(string.Format("Received restore purchases response. Error = {0}.", _error.GetPrintableString()));

        if (_transactions != null)
        {
            Debug.Log(string.Format("Count of transaction information received = {0}.", _transactions.Length));

            foreach (BillingTransaction _currentTransaction in _transactions)
            {
                this.GotProduct(_currentTransaction.ProductIdentifier);

                Debug.Log("Product Identifier = " + _currentTransaction.ProductIdentifier + "\n" +
                "Transaction State = " + _currentTransaction.TransactionState + "\n" +
                "Verification State = " + _currentTransaction.VerificationState + "\n" +
                "Transaction Date[UTC] = " + _currentTransaction.TransactionDateUTC + "\n" +
                "Transaction Date[Local] = " + _currentTransaction.TransactionDateLocal + "\n" +
                "Transaction Identifier = " + _currentTransaction.TransactionIdentifier + "\n" +
                "Transaction Receipt = " + _currentTransaction.TransactionReceipt + "\n" +
                "Error = " + _currentTransaction.Error.GetPrintableString());
            }
        }
    }

    public void BuyItem(string id)
    {
        Debug.Log("Buy item's button callback.");
        BillingProduct product = this.GetProductByID(id);
        if (product != null)
        {
            this.BuyItem(product);
        }
        else
        {
            Debug.Log("Cannot find ID : " + id);
        }
    }

    public void BuyItem(BillingProduct _product)
    {
        Debug.Log("Buy product : " + _product.ProductIdentifier);
        if (NPBinding.Billing.IsProductPurchased(_product.ProductIdentifier))
        {
            // Show alert message that item is already purchased
            Debug.Log("Alert : " + _product.ProductIdentifier + "has been purchased!");
            return;
        }

        // At this point you can display an activity indicator to inform user that task is in progress
        // if (m_LoadingLayer != null) m_LoadingLayer.SetVisible(true);

        // Call method to make purchase
        NPBinding.Billing.BuyProduct(_product);
        Debug.Log("Send purchase request.");
    }

    private void OnDidFinishTransaction(BillingTransaction _transaction)
    {
        Debug.Log("Receive purchase result.\n" +
            "Product Identifier = " + _transaction.ProductIdentifier + "\n" +
            "Transaction State = " + _transaction.TransactionState + "\n" +
            "Verification State = " + _transaction.VerificationState + "\n" +
            "Transaction Date[UTC] = " + _transaction.TransactionDateUTC + "\n" +
            "Transaction Date[Local] = " + _transaction.TransactionDateLocal + "\n" +
            "Transaction Identifier = " + _transaction.TransactionIdentifier + "\n" +
            "Transaction Receipt = " + _transaction.TransactionReceipt + "\n" +
            "Error = " + _transaction.Error.GetPrintableString());
        if (m_LoadingLayer != null) m_LoadingLayer.SetVisible(false);

        if (_transaction == null) return;

        if (_transaction.VerificationState == eBillingTransactionVerificationState.SUCCESS &&
            _transaction.TransactionState == eBillingTransactionState.PURCHASED)
        {
            this.GotProduct(_transaction.ProductIdentifier);
        }

    }

    public BillingProduct GetProductByID(string id)
    {
        if (m_Products.ContainsKey(id))
        {
            return m_Products[id];
        }

        return null;
    }

    private void GotProduct(string id)
    {
        // Debug.Log("Got product : " + id);
        if (m_DialogLayer != null)
        {
            m_DialogLayer.ShowText("Got product : \n" + id);
        }

        switch (id)
        {
            case Constants.ProductID_iOS_Coin_50:
                {
                    long curCoinNum = PlayerDataManager.instance.GetData_CoinNum();
                    PlayerDataManager.instance.SetData_CoinNum(curCoinNum + 50l);
                }
                break;

            case Constants.ProductID_iOS_Hero_01:
                {
                    PlayerDataManager.instance.SetData_SelectHero(1);
                }
                break;

            default:
                break;
        }
    }
}