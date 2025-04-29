# 🧠📊 AI-Powered Chart Generator using Syncfusion Toolkit for .NET MAUI

This project demonstrates how to use **natural language** and **AI (via Azure OpenAI)** to dynamically generate **interactive charts** in a .NET MAUI application, powered by the **Syncfusion Toolkit** for **.NET MAUI**.

It features a chatbot-like UI that accepts natural language input, generates chart data using Azure OpenAI, and displays it as a fully styled, interactive chart.

## 🚀 Features

- 🔸 Natural language input to request chart creation
- 🔸 AI response parsing using Azure OpenAI's GPT model
- 🔸 JSON-to-chart conversion via data binding
- 🔸 Built-in themes and templates using Syncfusion MAUI Toolkit
- 🔸 Uses `AiAssistView` to mimic ChatGPT-like experience
- 🔸 Chart export as image

## 📸 Preview

> Coming soon: Add a screenshot or screen recording of your app here.

## 🧰 Technologies Used

- [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Syncfusion .NET MAUI Toolkit](https://www.syncfusion.com/maui-controls)
- [Azure OpenAI Service](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/)
- VS Code (optional)

## 🧑‍💻 How It Works

1. User inputs a chart request in natural language.
2. A prompt is structured and sent to the Azure OpenAI service.
3. The AI returns a JSON string describing chart data and settings.
4. The app deserializes the JSON and binds it to Syncfusion chart controls.
5. The chart is rendered interactively using Syncfusion's visual components.

## 📝 Prerequisites

- [.NET 8 SDK with MAUI workload](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation)
- [Visual Studio Code](https://code.visualstudio.com/) or Visual Studio 2022+
- An active [Azure subscription](https://azure.microsoft.com/)
- [Access to Azure OpenAI](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/overview)

## 📦 Setup Instructions

1. **Clone this repository**
   ```bash
   git clone https://github.com/your-username/ai-powered-chart-generator.git
   cd ai-powered-chart-generator
