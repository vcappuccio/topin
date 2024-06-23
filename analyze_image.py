import argparse
import base64
import requests
import os


apiKey = "sk-proj-XXXX" # Replace with your OpenAI API key

def encode_image(image_path):
    with open(image_path, "rb") as image_file:
        return base64.b64encode(image_file.read()).decode('utf-8')

def analyze_image(image_path):
    base64_image = encode_image(image_path)

    headers = {
        "Content-Type": "application/json",
        "Authorization": f"Bearer {apiKey}"
    }

    payload = {
        "model": "gpt-4o",
        "messages": [
            {
                "role": "user",
                "content": [
                    {
                        "type": "text",
                        "text": "What's in this image?" # You can change this prompt
                    },
                    {
                        "type": "image_url",
                        "image_url": {
                            "url": f"data:image/jpeg;base64,{base64_image}"
                        }
                    }
                ]
            }
        ],
        "max_tokens": 300
    }

    response = requests.post("https://api.openai.com/v1/chat/completions", headers=headers, json=payload)
    return response.json()

def main(args):
    result = analyze_image(args.image)
    print(result['choices'][0]['message']['content'])

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Analyze an image using GPT-4o")
    parser.add_argument("image", help="Path to the image file")
    args = parser.parse_args()
    main(args)
