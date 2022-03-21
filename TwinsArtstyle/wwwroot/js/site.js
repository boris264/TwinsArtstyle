document.addEventListener("click", e => {
    const isDropDownButton = e.target.matches("[dropdown-button");

    if (!isDropDownButton && e.target.closest("[dropdown-menu]") != null) return;

    let dropdown;
    if (isDropDownButton) {
        dropdown = e.target.closest("[dropdown-menu]");
        dropdown.classList.toggle("active");
    }

    document.querySelectorAll("[dropdown-menu].active").forEach(item => {
        if (item === dropdown) return;
        item.classList.remove("active");
    })
})

const addToCartButtons = document.getElementsByClassName("add-to-cart-button");

for(let i = 0; i < addToCartButtons.length; i++)
{
    addToCartButtons[i].addEventListener("click", addToCart);
}

async function addToCart(event)
{
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

    if(response.ok)
    {
        const productImage = productCard.getElementsByClassName("product-image")[0].getAttribute("src");
        const productName = productCard.querySelector(".product-name").innerText;
        const productPrice = parseFloat(productCard.querySelector(".price").innerText.replace(" лв.", ""));
        const cartUl = document.querySelector(".cart-items").children[0];
        const cartItems = cartUl.children;

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

        const newProduct = createCartItem(productImage, productName, productPrice, productCount);
        cartUl.appendChild(newProduct);
    }
}

function createCartItem(image, name, price, quantity)
{
    let li = document.createElement('li');
    li.className = "cart-item";
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
    return li;
}