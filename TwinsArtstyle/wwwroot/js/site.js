document.addEventListener("click", e => {
    const isDropDownButton = e.target.matches("[dropdown-button");

    if (!isDropDownButton && e.target.closest("[dropdown-menu]") != null) return;

    let dropdown;

    if (isDropDownButton) {
        dropdown = e.target.closest("[dropdown-menu]");

        dropdown.classList.toggle("active");
    }

    if(!e.target.classList.contains("add-to-cart-button") && !e.target.classList.contains("remove-item"))
    {
        document.querySelectorAll("[dropdown-menu].active").forEach(item => {
            if (item === dropdown) return;
            item.classList.remove("active");
        })
    }
})

const placeOrderButtonContainer = document.getElementsByClassName("order-button-container")[0];
const totalPrice = document.getElementsByClassName("total-price")[0];
const addToCartButtons = document.getElementsByClassName("add-to-cart-button");

for(let i = 0; i < addToCartButtons.length; i++)
{
    addToCartButtons[i].addEventListener("click", addToCart);
}

const deleteItemButtons = document.getElementsByClassName("remove-item");

for(let i = 0; i < deleteItemButtons.length; i++)
{
    deleteItemButtons[i].addEventListener("click", deleteListItem);
}

async function addToCart(event)
{
    const cartDropdown = document.getElementsByClassName("cart-dropdown")[0];
    const productCard = event.target.closest(".product-card");
    const productCount = parseInt(productCard.querySelector(".product-count").value);
    const productCardAttributes = productCard.attributes;
    let productId = "";

    for(let i = 0; i < productCardAttributes.length; i++)
    {
        if(productCardAttributes[i].nodeName === "id")
        {
            productId = productCardAttributes[i].nodeValue;
        }
    }

    const product = {
        productId: productId,
        count: productCount
    };
    
    let response = await fetch("/Main/Cart/Add", 
    {
        method: "POST", 
        body: JSON.stringify(product),
        headers: {"Content-Type": "application/json"}
    });

    if(!response.redirected)
    {
        const productImage = productCard.getElementsByClassName("product-image")[0].getAttribute("src");
        const productName = productCard.querySelector(".product-name").innerText;
        const productPrice = parseFloat(productCard.querySelector(".price").innerText.replace(" лв.", ""));
        const cartUl = document.querySelector(".cart-items").children[0];
        const cartItems = cartUl.children;

        if(!cartDropdown.classList.contains("active"))
        {
            cartDropdown.classList.toggle("active");
        }
        

        for(let i = 0; i < cartItems.length; i++)
        {
            const itemName = cartItems[i].querySelector(".item-name").innerText;
            
            if(itemName === productName)
            {
                const oldQuantity = cartItems[i].getElementsByClassName("quantity")[0];
                const maxValue = parseInt(oldQuantity.getAttribute("max"));
                let newQuantity = parseInt(oldQuantity.value) + productCount;

                if(newQuantity > maxValue)
                {
                    newQuantity -= newQuantity - maxValue;
                }

                oldQuantity.setAttribute("value", newQuantity);
                return;
            }
        }
        
        const newProduct = createCartItem(productImage, productName, productPrice, productCount, productId);
        cartUl.appendChild(newProduct);

        if(cartItems.length > 0)
        {
            placeOrderButtonContainer.style.display = "block";
            totalPrice.style.display = "inline";
        }
    }
    else
    {
        window.location.replace(response.url);
    }
}

function createCartItem(image, name, price, quantity, productId)
{
    let li = document.createElement('li');
    li.classList.add("cart-item");
    li.id = productId;
    let productImage = document.createElement("img");
    productImage.src = image;
    productImage.className = "item-image";
    li.appendChild(productImage);
    let productName = document.createElement("span");
    productName.textContent = name;
    productName.className = "item-name";
    li.appendChild(productName);
    let productQuantity = document.createElement("input")
    productQuantity.type = "number";
    productQuantity.className = "quantity";
    productQuantity.setAttribute("value", quantity);
    productQuantity.max = 99;
    li.appendChild(productQuantity);
    const deleteIcon = document.createElement("i");
    deleteIcon.classList.add("bi");
    deleteIcon.classList.add("remove-item");
    deleteIcon.classList.add("bi-trash-fill");
    deleteIcon.addEventListener("click", deleteListItem);
    li.appendChild(deleteIcon);
    return li;
}

async function deleteListItem(e)
{
    const productId = e.target.closest(".cart-item").id;
    const product = {
        productId: productId
    }

    const response = await fetch("/Main/Cart/Remove", {
        method: "POST",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify(product)
    });

    if(response.ok)
    {
        const listItem = e.target.closest(".cart-item");
        const ul = e.target.closest(".items");
        ul.removeChild(listItem);

        if(ul.children.length === 0)
        {
            placeOrderButtonContainer.style.display = "none";
            totalPrice.style.display = "none";
        }
    }
}